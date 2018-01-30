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

namespace DapperUniversity.Controllers
{
    public class CoursesController : Controller
    {
        public const string DatabaseConnectionString = "host=172.17.0.2;port=5432;username=postgres;password=P@ssw0rd!;database=DapperUniversity;";

        [HttpGet]
        public async Task<IEnumerable<Course>> Index()
        {
            var query = @"SELECT * 
                      FROM course INNER JOIN department 
                      ON course.department_id = department.department_id;";
            // string builder?
            IEnumerable<Course> courses = Enumerable.Empty<Course>(); 
            using (DbContext _context = new DbContext(DatabaseConnectionString))
            {
                courses  = await _context.GetConnection().QueryAsync<Course> (query);
            }

            return courses;
        }

    }
}

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class InstructorIndexData
    {
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}
