using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;

namespace DapperUniversity.Controllers
{
    public class StudentsController : Controller
    {
        public const string DatabaseConnectionString = "host=172.17.0.2;port=5432;username=postgres;password=P@ssw0rd!;database=DapperUniversity;";

        [HttpGet]
        public async Task<IEnumerable<Student>> Index()
        {

            var sql = "SELECT last_name, first_name, enrollment_date FROM student";

            using (DbContext _context = new DbContext(DatabaseConnectionString))
            {

              return await _context.GetConnection().QueryAsync<Student>(sql);
            }
        }

        [HttpGet]
        public async Task<Student> Details(int? id)
        {

            var sql1 = "SELECT * from student WHERE student_id = @id";
            var sql2 = "SELECT * from enrollment WHERE student_id in ( SELECT student_id from student where student_id = @id)";
            var sql3 = "SELECT * FROM course WHERE course_id in (SELECT course_id from enrollment where student_id = @id)";

            using (DbContext _context = new DbContext(DatabaseConnectionString))
            {
              var student = await _context.GetConnection().QuerySingleAsync<Student> (sql1, new {id} );
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
    }
}
