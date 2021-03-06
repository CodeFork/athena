﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Core.Repositories;
using Athena.Data.Extensions;
using Dapper;

namespace Athena.Data.Repositories
{
    public class CourseRepository : PostgresRepository, ICourseRepository
    {
        public CourseRepository(IDbConnection db) : base(db)
        {
        }

        public async Task<Course> GetAsync(Guid id) =>
            (await _db.QueryAsync<Course, Institution, Course>(@"
                SELECT c.id,
                       c.name,
                       i.id,
                       i.name,
                       i.description
                FROM courses c
                    LEFT JOIN institutions i
                        ON c.institution = i.id
                WHERE c.id = @id",
                _mapInstitution,
                new {id}
            )).FirstOrDefault();

        public async Task AddAsync(Course obj) => await _db.InsertUniqueAsync(
            "INSERT INTO courses VALUES (@id, @name, @institution)",
            new {obj.Id, obj.Name, institution = obj.Institution.Id}
        );

        public async Task EditAsync(Course obj) => await _db.ExecuteAsync(@"
            UPDATE courses SET
                name = @name,
                institution = @institution
            WHERE id = @id",
            new {obj.Name, institution = obj.Institution.Id, obj.Id}
        );

        public async Task DeleteAsync(Course obj) => await _db.ExecuteAsync(
            "DELETE FROM courses WHERE id = @id",
            new {obj.Id}
        );

        public async Task<IEnumerable<Course>> GetCoursesForInstitutionAsync(Institution institution) =>
            await _db.QueryAsync<Course, Institution, Course>(@"
                SELECT c.id,
                       c.name,
                       i.id,
                       i.name,
                       i.description
                FROM courses c
                    LEFT JOIN institutions i
                        ON c.institution = i.id
                WHERE c.institution = @id",
                _mapInstitution,
                new {institution.Id}
            );

        public async Task<IEnumerable<Course>> GetCompletedCoursesForStudentAsync(Student student) =>
            await _db.QueryAsync<Course, Institution, Course>(@"
                SELECT c.id,
                       c.name,
                       i.id,
                       i.name,
                       i.description
                FROM courses c
                    LEFT JOIN institutions i
                        ON c.institution = i.id
                    LEFT JOIN student_x_completed_course link
                        ON c.id = link.course
                WHERE link.student = @student",
                _mapInstitution,
                new { student = student.Id }
            );

        public async Task MarkCourseAsCompletedForStudentAsync(Course course, Student student) =>
            await _db.InsertUniqueAsync(
                "INSERT INTO student_x_completed_course VALUES (@student, @course)",
                new {student = student.Id, course = course.Id}
            );

        public async Task MarkCourseAsUncompletedForStudentAsync(Course course, Student student) =>
            await _db.ExecuteAsync(
                "DELETE FROM student_x_completed_course WHERE student = @student AND course = @course",
                new {student = student.Id, course = course.Id}
            );

        public async Task<IEnumerable<Course>> GetInProgressCoursesForStudentAsync(Student student) =>
            await _db.QueryAsync<Course, Institution, Course>(@"
                SELECT c.id,
                       c.name,
                       i.id,
                       i.name,
                       i.description
                FROM courses c
                    LEFT JOIN institutions i
                        ON c.institution = i.id
                    LEFT JOIN student_x_in_progress_course link
                        ON c.id = link.course
                WHERE link.student = @student",
                _mapInstitution,
                new { student = student.Id }
            );

        public async Task MarkCourseInProgressForStudentAsync(Course course, Student student) =>
            await _db.InsertUniqueAsync(
                "INSERT INTO student_x_in_progress_course VALUES (@student, @course)",
                new {student = student.Id, course = course.Id}
            );

        public async Task MarkCourseNotInProgressForStudentAsync(Course course, Student student) =>
            await _db.ExecuteAsync(
                "DELETE FROM student_x_in_progress_course WHERE student = @student AND course = @course",
                new {student = student.Id, course = course.Id}
            );

        public async Task AddOfferingAsync(Course course, Offering offering) =>
            await _db.InsertUniqueAsync(
                "INSERT INTO course_x_offering VALUES (@course, @offering)",
                new {course = course.Id, offering = offering.Id}
            );

        public async Task RemoveOfferingAsync(Course course, Offering offering) =>
            await _db.ExecuteAsync(
                "DELETE FROM course_x_offering WHERE course = @course AND offering = @offering",
                new {course = course.Id, offering = offering.Id}
            );

        public async Task AddSatisfiedRequirementAsync(Course course, Requirement requirement) =>
            await _db.InsertUniqueAsync(
                "INSERT INTO course_requirements VALUES (@course, @requirement)",
                new {course = course.Id, requirement = requirement.Id}
            );

        public async Task RemoveSatisfiedRequirementAsync(Course course, Requirement requirement) =>
            await _db.ExecuteAsync(
                "DELETE FROM course_requirements WHERE course = @course AND requirement = @requirement",
                new {course = course.Id, requirement = requirement.Id}
            );

        public async Task AddPrerequisiteAsync(Course course, Requirement prereq) =>
            await _db.InsertUniqueAsync(
                "INSERT INTO course_prereqs VALUES (@course, @prereq)",
                new {course = course.Id, prereq = prereq.Id}
            );

        public async Task RemovePrerequisiteAsync(Course course, Requirement prereq) =>
            await _db.ExecuteAsync(
                "DELETE FROM course_prereqs WHERE course = @course AND prereq = @prereq",
                new {course = course.Id, prereq = prereq.Id}
            );
        
        public async Task AddConcurrentPrerequisiteAsync(Course course, Requirement prereq) =>
            await _db.InsertUniqueAsync(
                "INSERT INTO course_concurrent_prereqs VALUES (@course, @prereq)",
                new {course = course.Id, prereq = prereq.Id}
            );
        
        public async Task RemoveConcurrentPrerequisiteAsync(Course course, Requirement prereq) =>
            await _db.ExecuteAsync(
                "DELETE FROM course_concurrent_prereqs WHERE course = @course AND prereq = @prereq",
                new {course = course.Id, prereq = prereq.Id}
            );

        private static Course _mapInstitution(Course c, Institution i)
        {
            c.Institution = i;
            return c;
        }
    }
}