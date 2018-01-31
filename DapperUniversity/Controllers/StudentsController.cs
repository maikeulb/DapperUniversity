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

namespace DapperUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly string _connectionString;

        public StudentsController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<IEnumerable<Student>> Index(
            string sortOrder, 
            string currentFilter,
            string searchString,
            int? page)
        {

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            IEnumerable<Student> students = Enumerable.Empty<Student>(); 

            using (DbContext _context = new DbContext(_connectionString))
            {
                students = await _context.GetConnection().GetAllAsync<Student>();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
             students = students.Where(s => s.LastName.Contains(searchString)
                                   || s.FirstName.Contains(searchString));
            }
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
            int pageSize = 3;
            var pageNumber = (page ?? 1);

            return await PaginatedList<Student>.CreateAsync(students.AsQueryable(), pageNumber, pageSize);
        }

        [HttpGet]
        public async Task<Student> Details(int? id)
        {

            var sql2 = "SELECT * from enrollment WHERE student_id in ( SELECT student_id from student where student_id = @id)";
            var sql3 = "SELECT * FROM course WHERE course_id in (SELECT course_id from enrollment where student_id = @id)";

            using (DbContext _context = new DbContext(_connectionString))
            {
              var student = await _context.GetConnection().GetAsync<Student>(id);
              var enrollments = await _context.GetConnection().QueryAsync<Enrollment> (sql2, new {id} );
              var courses = await _context.GetConnection().QueryAsync<Course> (sql3, new {id} );

              student.Enrollments = enrollments.Where(e=>e.StudentId == student.StudentId).ToList();  //takes constructs enrollment in student graph

              foreach (var enrollment in student.Enrollments)
              {
                  enrollment.Course = courses.Where(c=>c.CourseId == enrollment.CourseId).Single();
              }

              return student;
            }
        }

        [HttpPost]
        public async Task Create([Bind("EnrollmentDate,FirstName,LastName")] Student student)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
              await _context.GetConnection().InsertAsync(student);
              return;
            }
        }

        [HttpPost]
        public async Task Edit(int? id)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
              var studentToUpdate = await _context.GetConnection().GetAsync<Student>(id);
              await _context.GetConnection().UpdateAsync(studentToUpdate);
              return;
            }
        }

        [HttpPost]
        public async Task Delete(int? id)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
              var studentToDelete = await _context.GetConnection().GetAsync<Student>(id);
              await _context.GetConnection().DeleteAsync(studentToDelete);
              return;
            }
        }
    }
}
