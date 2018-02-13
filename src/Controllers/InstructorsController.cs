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
using Microsoft.Extensions.Logging;

namespace DapperUniversity.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public InstructorsController(
            ILogger<InstructorsController> logger
                )
        {
            _connectionString = "Server=172.17.0.2;Port=5432;Database=DapperUniversity;User ID=postgres;Password=P@ssw0rd!;";
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int? id, int? courseId)
        {
            InstructorIndexData viewModel = new InstructorIndexData();

            string query = @"SELECT i.*, oa.*
                             FROM instructors AS i
                               LEFT JOIN office_assignments AS oa 
                               ON oa.instructor_id = i.id;";

            using (DbContext _context = new DbContext(_connectionString))
            {
                viewModel.Instructors = await _context.GetConnection().QueryAsync<Instructor, OfficeAssignment, Instructor> (query,
                    ((instructor, assignment) =>
                    {
                        instructor.OfficeAssignment = assignment;
                        return instructor;
                    }), splitOn: "instructor_id");
            }

            viewModel.Instructors.ToList().ForEach(i =>
            {
                GetInstructorCourse(i.Id).ToList().ForEach(i.AddCourse);

            });

            if (id != null)
            {
                ViewBag.InstructorId = id.Value;

                query = @"SELECT c.id, c.title, d.*
                          FROM courses AS c 
                            INNER JOIN departments AS d 
                            ON d.id = c.department_id 
                            INNER JOIN course_assignments AS ca 
                            ON ca.course_id = c.id 
                          WHERE ca.instructor_id = @id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                viewModel.Courses = await _context.GetConnection().QueryAsync<Course, Department, Course> (query,
                    ((course, department) =>
                    {
                        course.Department = department;
                        return course;
                    }),
                    new { id });
            }
            }

            if (courseId != null)
            {
                ViewBag.CourseId = courseId.Value;

                query = @"SELECT e.grade, s.*
                          FROM enrollments AS e 
                            INNER JOIN students AS s 
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
                    new { courseId });
            }
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            Instructor instructor = await GetInstructorWithCourse(id);
            return View(instructor);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var instructor = new Instructor() { };
            instructor.CourseAssignments = new List<CourseAssignment>();
            await PopulateAssignedCourseData(instructor);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind(" FirstName,HireDate,LastName,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.CourseAssignments = new List<CourseAssignment>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignment { InstructorId = instructor.Id, CourseId = int.Parse(course) };
                    instructor.CourseAssignments.Add(courseToAdd);
                }
            }

            string commandInstructor = @"INSERT INTO instructors (last_name, first_name, hire_date) 
                                         VALUES(@LastName, @FirstName, @HireDate);
                                         INSERT INTO office_assignments (instructor_id, location)
                                         VALUES (currval('instructors_id_seq'), @Location)";
            if (ModelState.IsValid)
            {
                using (DbContext _context = new DbContext(_connectionString))
                {
                    await _context.GetConnection().ExecuteAsync(commandInstructor, new{ 
                            instructor.LastName, 
                            instructor.FirstName, 
                            instructor.HireDate, 
                            instructor.OfficeAssignment.Location});
                    return RedirectToAction("Index");
                }
            }

            await PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            var instructors = await GetInstructor(id);
            await PopulateAssignedCourseData(instructors);
            return View(instructors);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id, string[] selectedCourses)
        {
            Instructor instructorToUpdate = new Instructor();

            instructorToUpdate = await GetInstructor(id);
            var courses = GetInstructorCourse(id);
            courses.ToList().ForEach(instructorToUpdate.AddCourse);
            instructorToUpdate.OfficeAssignment.InstructorId = instructorToUpdate.Id;

            string command = @"UPDATE instructors 
                               SET first_name = @FirstName, 
                                   last_name = @LastName,
                                   hire_date = @HireDate
                               WHERE id = @Id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                if (await TryUpdateModelAsync<Instructor>(
                    instructorToUpdate,
                    "",
                    i => i.FirstName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
                {

                    instructorToUpdate.UpdateInstructorCourses(selectedCourses, courses.ToList());
                    await _context.GetConnection().ExecuteAsync(command, instructorToUpdate);
                    return RedirectToAction(nameof(Index));
                }
                PopulateAssignedCourseData(instructorToUpdate);
                return View(instructorToUpdate);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete (int? id)
        {
            Instructor instructor = await GetInstructorWithCourse(id);

            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeletePost (int? id)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                var instructorToDelete = await _context.GetConnection().GetAsync<Instructor>(id);
                await _context.GetConnection().DeleteAsync(instructorToDelete);
            }
            return RedirectToAction("Index"); 
        } 

        private IEnumerable<Course> GetInstructorCourse(int? id)
        {
            IEnumerable<Course> courses = Enumerable.Empty<Course>();


            string query =@"SELECT c.* 
                            FROM courses c
                              INNER JOIN course_assignments ca 
                              ON ca.course_id = c.Id
                            WHERE ca.instructor_id = @id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                courses = _context.GetConnection().Query<Course, CourseAssignment, Course> (query,
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
            IEnumerable<Instructor> instructors = Enumerable.Empty<Instructor>();

            string query = @"SELECT i.*, oa.*
                             FROM instructors AS i 
                               LEFT JOIN office_assignments AS oa 
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
                    splitOn: "location");
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

            ViewData["Courses"] = viewModel;
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
