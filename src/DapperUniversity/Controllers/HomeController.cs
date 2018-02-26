using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperUniversity.Data;
using DapperUniversity.Models;
using DapperUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DapperUniversity.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public HomeController(ILogger<HomeController> logger,
                IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString ("DapperUniversity");
        }

        public IActionResult Index ()
        {
            return View ();
        }

        [HttpGet]
        public async Task<IActionResult> About()
        {
            IEnumerable<EnrollmentDateGroupViewModel> groups = Enumerable.Empty<EnrollmentDateGroupViewModel> ();

            var students = Enumerable.Empty<Student> ();

            using (DbContext _context = new DbContext(_connectionString))
            {
                students = await _context.GetConnection ().GetAllAsync<Student> ();
            }

            groups = students.GroupBy (s => s.EnrollmentDate)
                .Select (x => new EnrollmentDateGroupViewModel
                {
                    EnrollmentDate = x.Key,
                    StudentCount = x.Count ()
                });

            return View(groups);
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
