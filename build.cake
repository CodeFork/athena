#addin "nuget:?package=Cake.Docker&version=0.9.0"
#addin "nuget:?package=Cake.NPM&version=0.13.0"
#addin "nuget:?package=Cake.Codecov&version=0.3.0"
#addin "nuget:?package=Cake.MiniCover&version=0.26.6"

#tool "nuget:?package=Codecov&version=1.0.3"

#l "./build/AddMigration.cake"

SetMiniCoverToolsProject("./minicover/minicover.csproj");

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

const string SOLUTION = "./Athena.sln";

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var imageTag = Argument("imageTag", "latest");
var coverageThreshold = Argument("coverageThreshold", 60.0f);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean::Dist")
    .Does(() => 
{
    if(DirectoryExists("./_dist"))
    {
        DeleteDirectory("./_dist", new DeleteDirectorySettings
        {
            Recursive = true
        });
    }
});

Task("Clean::Test")
    .Does(() => 
{
    DeleteFiles("*coverage*");
});

Task("Clean")
    .IsDependentOn("Clean::Dist")
    .IsDependentOn("Clean::Test")
    .Does(() =>
{
    DotNetCoreClean(SOLUTION);
});

Task("Restore::NPM")
    .Does(() =>
{
    NpmInstall(new NpmInstallSettings
    {
        WorkingDirectory = "./src/Athena"
    });
});

Task("Restore::Nuget")
    .Does(() =>
{
    DotNetCoreRestore(SOLUTION);
});

Task("Restore")
    .IsDependentOn("Restore::NPM")
    .IsDependentOn("Restore::Nuget");

Task("Bundle")
    .IsDependentOn("Restore::NPM")
    .Does(() => 
{
    NpmRunScript(new NpmRunScriptSettings
    {
        WorkingDirectory = "./src/Athena",
        ScriptName = configuration == "Release" ? "bundle"  : "bundle::dev"
    });
});

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Bundle")
    .Does(() =>
{
    DotNetCoreBuild(SOLUTION, new DotNetCoreBuildSettings {
        Configuration = configuration,
        NoRestore = true
    });
});

Task("Test::Prepare")
    .IsDependentOn("Clean::Test")
    .IsDependentOn("Build")
    .Does(() => 
{
    MiniCoverInstrument(
        new MiniCoverSettings()
            .WithAssembliesMatching("./test/**/*.dll")
            .WithSourcesMatching("./src/**/*.cs")
    );
    MiniCoverReset();
});

Task("Test::Unit")
    .IsDependentOn("Test::Prepare")
    .Does(() =>
{
    foreach(var project in GetFiles("./test/Unit/**/*.csproj"))
    {
        DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true
        });
    }
});

Task("Test::Integration")
    .IsDependentOn("Test::Prepare")
    .Does(() => 
{
    var container = Guid.Empty;
    if(HasArgument("UseDocker"))
    {
        DockerRun(new DockerContainerRunSettings
        {
            Name = (container = Guid.NewGuid()).ToString(),
            Detach = true,
            Publish = new [] {"5432:5432"},
            Rm = true
        }, "postgres:9.6-alpine", null, null);
        Information("Waiting 10 seconds to allow postgres to start...");
        System.Threading.Thread.Sleep(10000);
    }

    try
    {
        foreach(var project in GetFiles("./test/Integration/**/*.csproj"))
        {
            DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings
            {
                Configuration = configuration,
                NoRestore = true,
                NoBuild = true
            });
        }
    }
    finally
    {
        if(container != Guid.Empty)
        {
            DockerStop(container.ToString());
        }
    }
});

Task("Test")
    .IsDependentOn("Test::Unit")
    .IsDependentOn("Test::Integration")
    .Does(() =>
{
    MiniCoverUninstrument();

    Information("Coverage Results: ");
            
    MiniCoverReport(
        new MiniCoverSettings()
            .GenerateReport(ReportType.CONSOLE | ReportType.OPENCOVER)
            .WithThreshold(coverageThreshold)
            .WithNonFatalThreshold()
    );
});

Task("Dist")
    .IsDependentOn("Clean::Dist")
    .IsDependentOn("Test")
    .Does(() => 
{
    EnsureDirectoryExists("./_dist");

    DotNetCorePublish("./src/Athena/Athena.csproj", new DotNetCorePublishSettings
    {
        Configuration = configuration,
        NoRestore = true,
        OutputDirectory = MakeAbsolute(Directory("./_dist/Athena")).FullPath
    });
});

Task("Docker::Build")
    .IsDependentOn("Dist")
    .Does(() =>
{
    DockerBuild(new DockerImageBuildSettings
    {
        Tag = new []{$"athenascheduler/athena:{imageTag}"}
    }, ".");
});

Task("Docker::Push")
    .IsDependentOn("Docker::Build")
    .Does(() =>
{
    DockerPush($"athenascheduler/athena:{imageTag}");
});

Task("CodeCov::Publish")
    .IsDependentOn("Test")
    .Does(() =>
{
    Codecov("./coverage-opencover.xml");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);