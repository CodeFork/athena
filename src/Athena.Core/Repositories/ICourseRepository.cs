﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Athena.Core.Models;

namespace Athena.Core.Repositories
{
    public interface ICourseRepository : IRepository<Course, Guid>
    {
        /// <summary>
        /// Get all courses offered by the provided institution
        /// </summary>
        /// <param name="institution">The institution to get courses for</param>
        /// <returns>An IEnumerable of Courses</returns>
        Task<IEnumerable<Course>> GetCoursesForInstitutionAsync(Institution institution);

        /// <summary>
        /// Get all completed courses that the provided student has taken 
        /// </summary>
        /// <param name="student">The student to get completed courses for</param>
        /// <returns>An IEnumerable of Courses</returns>
        Task<IEnumerable<Course>> GetCompletedCoursesForStudentAsync(Student student);

        /// <summary>
        /// Mark the specified course as completed for the provided student
        /// </summary>
        /// <param name="course">The course the student has completed</param>
        /// <param name="student">The student who completed the course</param>
        Task MarkCourseAsCompletedForStudentAsync(Course course, Student student);

        /// <summary>
        /// Mark the specified course as uncompleted for the provided student
        /// </summary>
        /// <param name="course">The course to mark as not completed</param>
        /// <param name="student">The student to modify</param>
        Task MarkCourseAsUncompletedForStudentAsync(Course course, Student student);
        
        /// <summary>
        /// Get all in-progress courses for the specified students
        /// </summary>
        /// <param name="student">The student to get courses for</param>
        /// <returns>An IEnumerable of Courses</returns>
        Task<IEnumerable<Course>> GetInProgressCoursesForStudentAsync(Student student);

        /// <summary>
        /// Mark the specified course as in-progress for the specified student
        /// </summary>
        /// <param name="course">The course to mark as in-progress</param>
        /// <param name="student">The student who is taking the course</param>
        Task MarkCourseInProgressForStudentAsync(Course course, Student student);
        
        /// <summary>
        /// Mark the specified course as not in-progress for the specified student
        /// </summary>
        /// <param name="course">The course to mark as not in-progress</param>
        /// <param name="student">The student who is taking the course</param>
        Task MarkCourseNotInProgressForStudentAsync(Course course, Student student);

        /// <summary>
        /// Add an offering to the specified course
        /// </summary>
        /// <param name="course">The course to add the offering to</param>
        /// <param name="offering">The offering to add</param>
        Task AddOfferingAsync(Course course, Offering offering);
        
        /// <summary>
        /// Remove an offering from the specified course
        /// </summary>
        /// <param name="course">The course to remove the offering from</param>
        /// <param name="offering">The offering to remove</param>
        Task RemoveOfferingAsync(Course course, Offering offering);

        /// <summary>
        /// Add a requirement that this course satisfies
        /// </summary>
        /// <param name="course">The course to modify</param>
        /// <param name="requirement">The requirement that the course satisfies</param>
        Task AddSatisfiedRequirementAsync(Course course, Requirement requirement);
        
        /// <summary>
        /// Remove a requirement that this course satisfies
        /// </summary>
        /// <param name="course">The course to modify</param>
        /// <param name="requirement">The requirement to remove</param>
        Task RemoveSatisfiedRequirementAsync(Course course, Requirement requirement);

        /// <summary>
        /// Add a prerequisite for the specified course
        /// </summary>
        /// <param name="course">The course to modify</param>
        /// <param name="prereq">The prerequisite to add</param>
        Task AddPrerequisiteAsync(Course course, Requirement prereq);
        
        /// <summary>
        /// Remove a prerequisite for the specified course
        /// </summary>
        /// <param name="course">The course to modify</param>
        /// <param name="prereq">The prerequisite to remove</param>
        Task RemovePrerequisiteAsync(Course course, Requirement prereq);
        
        /// <summary>
        /// Add a concurrent prerequisite for the specified course
        /// </summary>
        /// <param name="course">The course to modify</param>
        /// <param name="prereq">The concurrent prerequisite to add</param>
        Task AddConcurrentPrerequisiteAsync(Course course, Requirement prereq);
        
        /// <summary>
        /// Remove a concurrent prerequisite for the specified course
        /// </summary>
        /// <param name="course">The course to modify</param>
        /// <param name="prereq">The concurrent prerequisite to remove</param>
        Task RemoveConcurrentPrerequisiteAsync(Course course, Requirement prereq);
    }
}