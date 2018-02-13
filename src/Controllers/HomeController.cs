using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using DapperUniversity.Data;
using DapperUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace DapperUniversity.Controllers
{
    public class HomeController : Controller
    {
        public const string DatabaseConnectionString = "host=172.17.0.2;port=5432;username=postgres;password=P@ssw0rd!;database=DapperUniversity;";

        public IActionResult Index ()
        {
            return View ();
        }

        [HttpGet]
        public async Task<IEnumerable<EnrollmentDateGroup>> About ()
        {

            IEnumerable<EnrollmentDateGroup> result = Enumerable.Empty<EnrollmentDateGroup> ();

            var students = Enumerable.Empty<Student> ();

            using (DbContext _context = new DbContext (DatabaseConnectionString))
            {
                students = await _context.GetConnection ().GetAllAsync<Student> ();
            }

            result = students.GroupBy (s => s.EnrollmentDate)
                .Select (x => new EnrollmentDateGroup
                {
                    EnrollmentDate = x.Key,
                        StudentCount = x.Count ()
                });

            return result;
        }

        public IActionResult Contact ()
        {
            ViewData["Message"] = "Your contact page.";

            return View ();
        }

        public IActionResult Error ()
        {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

namespace DapperUniversity.Models
{
    public class EnrollmentDateGroup
    {
        public DateTime? EnrollmentDate { get; set; }

        public int StudentCount { get; set; }
    }
}