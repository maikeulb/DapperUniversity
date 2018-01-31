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
        private readonly string _connectionString;

        public CoursesController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<IEnumerable<Course>> Index(int? id, int? courseId)
        {
            IEnumerable<Course> courses = Enumerable.Empty<Course>(); 
            var query = @"SELECT c.*, d.name 
                          FROM course c 
                            INNER JOIN department 
                            ON d.department_id = c.department_id;";

            using (DbContext _context = new DbContext(_connectionString))
            {
                courses  = await _context.GetConnection().QueryAsync<Course, Department, Course> (query,
                    (courseItem, deparment) =>
                    {
                        courseItem.Department = deparment;
                        return courseItem;
                    },
                        splitOn: "department_id");
            }

            return courses;
        }

    }
}
