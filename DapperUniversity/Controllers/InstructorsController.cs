using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;

namespace DapperUniversity.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly string _connectionString;

        public InstructorsController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<InstructorIndexData> Index(int? id, int? courseId)
        {
            string query = string.Empty;

            InstructorIndexData viewModel = new InstructorIndexData();

            query = @"SELECT i.* oa.location 
                      FROM instuctor AS i
                        LEFT JOIN office_assignment AS oa 
                        ON oa.instructor_id = i.instructor_id;";

            using (DbContext _context = new DbContext(_connectionString))
            {
                viewModel.Instructors = await _context.GetConnection().QueryAsync<Instructor, OfficeAssignment, Instructor> (query,
                    ((instructor, assignment) =>
                    {
                        instructor.OfficeAssignment = assignment;
                        return instructor;
                    }), splitOn: "id");
            }

            viewModel.Instructors.ToList().ForEach(i =>
            {
                GetInstructorCourse(i.Id).ToList().ForEach(i.AddCourse);
            });

            if (id != null)
            {
                ViewBag.InstructorId = id.Value;

                query = @"SELECT c.id, c.title, d.name 
                          FROM course AS c 
                            INNER JOIN department AS d 
                            ON d.department_id = c.department_id 
                            INNER JOIN course_instructor AS ci 
                            ON ci.course_id = c.id 
                          WHERE ci.instructor_id = @id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                viewModel.Courses = await _context.GetConnection().QueryAsync<Course, Department, Course> (query,
                    ((course, department) =>
                    {
                        course.Department = department;
                        return course;
                    }),
                    new { id },
                    splitOn: "id");
            }
            }

            if (courseId != null)
            {
                ViewBag.CourseId = courseId.Value;

                query = @"SELECT e.grade, s.last_name, s.first_name 
                          FROM enrollment AS e 
                            INNER JOIN student AS s 
                            ON s.id = e.student_id 
                          WHERE e.course_id = @courseId";

            using (DbContext _context = new DbContext(_connectionString))
            {
                viewModel.Enrollments = await _context.GetConnection().QueryAsync<Enrollment, Student, Enrollment>(query,
                    ((enrollment, student) =>
                    {
                        enrollment.Student = student;
                        return enrollment;
                    }),
                    new { courseId },
                    splitOn: "id");
            }
            }

            return viewModel;
        }

        [HttpGet]
        public async Task<Instructor> Details(int? id)
        {
            return await GetInstructorWithCourse(id);
        }

        [HttpGet]
        public void Create()
        {
            return;
        }

        [HttpPost]
        public async Task Create ([Bind("LastName,FirstName,HireDate")] Instructor instructor)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                await _context.GetConnection().InsertAsync(instructor);
            }
            return; 
        }

        private IEnumerable<Course> GetInstructorCourse(int? id)
        {
            IEnumerable<Course> courses = Enumerable.Empty<Course>();

            string query =@"SELECT c.* 
                            FROM course c
                              INNER JOIN course_instructor ci 
                              ON ci.course_id = c.Id
                            WHERE ci.instructor_id = @id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                courses = _context.GetConnection().Query<Course, CourseAssignment, Course> (query,
                    ((course, courseAssignment) => course), 
                    new { id }, 
                    splitOn: "id");
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
            IEnumerable<Instructor> instructors = Enumerable.Empty<Instructor>();

            string query = @"SELECT i.*, oa.location 
                             FROM instructor AS i 
                               LEFT JOIN office_assignment AS oa 
                               ON i.id = oa.instructor_id 
                             WHERE i.id = @id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                instructors = await _context.GetConnection().QueryAsync<Instructor, OfficeAssignment, Instructor> (query,
                    ((instructorItem, assignment) =>
                    {
                        instructorItem.OfficeAssignment = assignment;
                        return instructorItem;
                    }),
                    new { id },
                    splitOn: "id");
            }

            return instructors.First();
        }

        private async Task PopulateAssignedCourseData(Instructor instructor)
        {
            IEnumerable<Course> allCourses = Enumerable.Empty<Course>();
            using (DbContext _context = new DbContext(_connectionString))
            {
                allCourses = await _context.GetConnection().GetAllAsync<Course>();
            }
            var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(s => s.CourseId));
            var viewModel = allCourses.Select(course => new AssignedCourseData
            {
                CourseId = course.Id,
                Title = course.Title,
                Assigned = instructorCourses.Contains(course.Id)
            }).ToList();
            ViewBag.AssignedCourses = viewModel;
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

    public class AssignedCourseData
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }
    }
}
