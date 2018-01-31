using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using ExtensionMethods;

namespace DapperUniversity.Controllers
{
    public class InstructorsController : Controller
    {
        public const string DatabaseConnectionString = "host=172.17.0.2;port=5432;username=postgres;password=P@ssw0rd!;database=DapperUniversity;";

        [HttpGet]
        public async Task<InstructorIndexData> Index(int? id, int? courseId)
        {
            string query = string.Empty;

            InstructorIndexData viewModel = new InstructorIndexData();

            query = @"SELECT i.* oa.location 
                      FROM location instuctor i
                        LEFT JOIN office_assignment oa 
                        ON oa.instructor_id = i.instructor_id;";

            using (DbContext _connection = new DbContext(DatabaseConnectionString))
            {
                viewModel.Instructors = await _connection.GetConnection().QueryAsync<Instructor, OfficeAssignment, Instructor> (query,
                    ((instructor, assignment) =>
                    {
                        instructor.OfficeAssignment = assignment;
                        return instructor;
                    }), splitOn: "instructor_id");
            }

            /* IEnumerable<Course> courses = Enumerable.Empty<Course>(); */
            /* var tasks = viewModel.Instructors.ToList().Select(s => */ 
                /* GetInstructorCourse(s.InstructorId)); */
            /* var courses = await Task.WhenAll(tasks); */


            viewModel.Instructors.ToList().ForEach(s =>
            {
                GetInstructorCourse(s.InstructorId).ToList().ForEach(s.AddCourse);
            });

            if (id != null)
            {
                ViewBag.InstructorId = id.Value;

                query = @"SELECT c.course_id, c.title, d.name 
                          FROM course c 
                            INNER JOIN department d 
                            ON d.department_id = c.department_id 
                            INNER JOIN course+instructor ci 
                            ON ci.course_id = c.course_id 
                          WHERE ci.instructor_id = @id";

            using (DbContext _connection = new DbContext(DatabaseConnectionString))
            {
                viewModel.Courses = await _connection.GetConnection().QueryAsync<Course, Department, Course> (query,
                    ((course, department) =>
                    {
                        course.Department = department;
                        return course;
                    }),
                    new { id },
                    splitOn: "department_id");
            }
            }

            if (courseId != null)
            {
                ViewBag.CourseId = courseId.Value;

                query = @"SELECT e.grade, s.last_name, s.first_name 
                          FROM enrollment e 
                            INNER JOIN student s 
                            ON s.student_id = e.student_id 
                          WHERE e.course_id = @courseId";

            using (DbContext _connection = new DbContext(DatabaseConnectionString))
            {
                viewModel.Enrollments = await _connection.GetConnection().QueryAsync<Enrollment, Student, Enrollment>(query,
                    ((enrollment, student) =>
                    {
                        enrollment.Student = student;
                        return enrollment;
                    }),
                    new { courseId },
                    splitOn: "student_id");
            }
            }

            return viewModel;
        }

        private IEnumerable<Course> GetInstructorCourse(int? id)
        {
            IEnumerable<Course> courses = Enumerable.Empty<Course>();
            /* List<Course> courses = new List<Course>(); */

            string query =@"SELECT c.* 
                            FROM course c
                              INNER JOIN course_instructor ci 
                              ON ci.course_id = c.CourseId
                            WHERE ci.instructor_id = @id";

            using (DbContext _connection = new DbContext(DatabaseConnectionString))
            {
                courses = _connection.GetConnection().Query<Course, CourseAssignment, Course> (query,
                    ((course, courseAssignment) => course), 
                    new { id }, 
                    splitOn: "department_id");
             }

            return courses;

        }

        private async Task<Instructor> GetInstructorWithCourse(int? id)
        {
            Instructor instructor = await GetInstructor(id);

            IEnumerable<Course> course = GetInstructorCourse(id); 

            course.ToList().ForEach(s =>
            {
                instructor.AddCourse(s);
            });
            return instructor;
        }

        private async Task<Instructor> GetInstructor(int? id)
        {
            /* Instructor instructor = new Instructor(); */
            IEnumerable<Instructor> instructors = Enumerable.Empty<Instructor>();// hacky, for some reason queryfirst isn't working

            string query = @"SELECT i.*, o.location 
                             FROM instructor i 
                               LEFT JOIN office_assignment o 
                               ON i.instructor_id = o.instructor_id 
                             WHERE i.instructor_id = @id";

            using (DbContext _connection = new DbContext(DatabaseConnectionString))
            {
                instructors = await _connection.GetConnection().QueryAsync<Instructor, OfficeAssignment, Instructor> (query,
                    ((instruct, assignment) =>
                    {
                        instruct.OfficeAssignment = assignment;
                        return instruct;
                    }),
                    new { id },
                    splitOn: "Location");
            }

            return instructors.First();
        }
    }

}

namespace DapperUniversity.Models
{
    public class InstructorIndexData
    {
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }

}

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }
    }   
}
