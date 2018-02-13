using System;
using System.Data;
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
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DapperUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public StudentsController(
            ILogger<InstructorsController> logger
                )
        {
            _connectionString = "Server=172.17.0.2;Port=5432;Database=DapperUniversity;User ID=postgres;Password=P@ssw0rd!;";
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Index(
            string sortOrder, 
            string currentFilter,
            string searchString,
            int? page)
        {
            IEnumerable<Student> students = Enumerable.Empty<Student>(); 

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            using (DbContext _context = new DbContext(_connectionString))
            {
                students = await _context.GetConnection().GetAllAsync<Student>();
            }

            if (!String.IsNullOrEmpty(searchString))
               students = students.Where(s => 
                     s.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                  || s.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0 );

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            return View(students);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            Student student = new Student();

            string enrollmentQuery = @"SELECT * 
                                       FROM enrollments
                                       WHERE student_id 
                                       IN (SELECT id 
                                         FROM students
                                         WHERE id = @id)";

            string courseQuery = @"SELECT * 
                                   FROM courses
                                   WHERE id 
                                   IN (SELECT course_id 
                                     FROM enrollments 
                                     WHERE student_id = @id)";

            using (DbContext _context = new DbContext(_connectionString))
            {
                /* var students = _context.GetConnection().QueryMultiple(enrollmentQuery, new { id }); */
                student = await _context.GetConnection().GetAsync<Student>(id);
                var enrollments = await _context.GetConnection().QueryAsync<Enrollment> (enrollmentQuery, new {id} );
                var courses = await _context.GetConnection().QueryAsync<Course> (courseQuery, new {id} );
  
                student.Enrollments = enrollments.Where(e=>e.StudentId == student.Id).ToList();  

                foreach (var enrollment in student.Enrollments)
                {
                    enrollment.Course = courses.Where(c=>c.Id == enrollment.CourseId).Single();
                }
            }

        return View(student);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create ([Bind("EnrollmentDate,FirstName,LastName")] Student student)
        {
            string command = @"INSERT INTO students (enrollment_date, first_name, last_name) 
                               VALUES(@EnrollmentDate, @FirstName, @LastName)";

            using (DbContext _context = new DbContext(_connectionString))
            {
                await _context.GetConnection().ExecuteAsync(command, student);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            Student student = new Student();

            using (DbContext _context = new DbContext(_connectionString))
            {
                student = await _context.GetConnection().GetAsync<Student>(id);
            }
            return View(student);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<ActionResult> EditPost (int id)
        {
            Student student = new Student();

            string command = @"UPDATE students 
                               SET enrollment_date = @EnrollmentDate, 
                                   first_name = @FirstName,
                                   last_name = @LastName
                               WHERE id = @Id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                student = await _context.GetConnection().GetAsync<Student>(id);
                if (await TryUpdateModelAsync<Student>(
                    student,
                    "",
                    s => s.FirstName, s => s.LastName, s => s.EnrollmentDate))
                {
                    await _context.GetConnection().ExecuteAsync(command, student);
                }
                return RedirectToAction("Index");
            }
            return View(student); 
        }

        [HttpGet]
        public async Task<ActionResult> Delete (int? id)
        {
            Student student = new Student();

            using (DbContext _context = new DbContext(_connectionString))
            {
                student = await _context.GetConnection().GetAsync<Student>(id);
            }

            return View(student); 
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeletePost (int? id)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                var studentToDelete = await _context.GetConnection().GetAsync<Student>(id);
                await _context.GetConnection().DeleteAsync(studentToDelete);
            }
            return RedirectToAction("Index"); 
        }
    }
}
