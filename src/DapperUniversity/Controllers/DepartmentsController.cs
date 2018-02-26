using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using DapperUniversity.Data;
using DapperUniversity.Models;
using DapperUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DapperUniversity.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public DepartmentsController(ILogger<DepartmentsController> logger,
                IConfiguration configuration)
        {
             _logger = logger;
             _connectionString = configuration.GetConnectionString ("DapperUniversity");
        }

        public async Task<ActionResult> Index ()
        {

            IEnumerable<Course> courses = Enumerable.Empty<Course> ();
            IEnumerable<Department> departments = Enumerable.Empty<Department> ();

            string query = @"SELECT d.*, i.*
                             FROM departments d
                               INNER JOIN instructors i
                               ON i.id = d.instructor_id";

            using (DbContext _context = new DbContext (_connectionString))
            {
                var _connection = _context.GetConnection ();
                _connection.Open ();
                courses = await _connection.GetAllAsync<Course> ();
                departments = await _connection.QueryAsync<Department, Instructor, Department> (query,
                    ((department, instructor) =>
                    {
                        department.Instructor = instructor;
                        return department;
                    }));
            }

            return View (departments.ToList ());
        }

        public async Task<ActionResult> Details (int? id)
        {
            if (id == null)
                return NotFound();

            string query = @"SELECT d.*, i.*
                             FROM departments d
                             INNER JOIN instructors i
                               ON i.id = d.instructor_id
                               WHERE d.id = @id";

            IEnumerable<Department> departments = Enumerable.Empty<Department> ();
            using (DbContext _context = new DbContext (_connectionString))
            {
                departments = await _context.GetConnection ().QueryAsync<Department, Instructor, Department> (query,
                    ((department, instructor) =>
                    {
                        department.Instructor = instructor;
                        return department;
                    }),
                    new { id });
            }

            if (departments == null)
                return NotFound();

            return View (departments.FirstOrDefault ());
        }

        public IActionResult Create ()
        {
            PopulateInstructorDepartmentList ();

            return View ();
        }

        [HttpPost]
        public async Task<ActionResult> Create ([Bind ("Name, Budget, InstructorId, StartDate")] Department department)
        {
            if (ModelState.IsValid)
            {
                string command = @"INSERT INTO departments (name, budget, instructor_id, start_date)
                               VALUES(@Name, @Budget, @InstructorId, @StartDate)";

                using (DbContext _context = new DbContext (_connectionString))
                {
                    await _context.GetConnection().ExecuteAsync (command, department);
                }

                return RedirectToAction ("Index");
            }

            PopulateInstructorDepartmentList (department.Id);

            return View ();
        }

        public async Task<ActionResult> Edit (int? id)
        {
            if (id == null)
                return NotFound();

            Department department = new Department ();

            using (DbContext _context = new DbContext (_connectionString))
            {
                department = await _context.GetConnection ().GetAsync<Department> (id);
            }

            if (department == null)
                return NotFound();

            PopulateInstructorDepartmentList (department.Id);

            return View (department);
        }

        [HttpPost, ActionName ("Edit")]
        public async Task<ActionResult> EditPost (int id)
        {
            if (id == null)
                return NotFound();

            Department departmentToUpdate = new Department ();

            string command = @"UPDATE departments
                               SET instructor_id = @InstructorId,
                                   name = @Name,
                                   budget = @Budget,
                                   start_date = @StartDate
                               WHERE id = @Id";

            using (DbContext _context = new DbContext (_connectionString))
            {
                departmentToUpdate = await _context.GetConnection ().GetAsync<Department> (id);
                if (await TryUpdateModelAsync<Department> (
                    departmentToUpdate,
                    "",
                    s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorId))
                {
                    await _context.GetConnection ().ExecuteAsync (command, departmentToUpdate);
                    return RedirectToAction("Index");
                }
            }

            PopulateInstructorDepartmentList (departmentToUpdate.Id);
            return View (departmentToUpdate);
        }

        public async Task<ActionResult> Delete (int? id)
        {
            if (id == null)
                return NotFound();

            Department department = new Department ();

            using (DbContext _context = new DbContext (_connectionString))
            {
                department = await _context.GetConnection ().GetAsync<Department> (id);
            }

            if (department == null)
                return NotFound();

            return View (department);
        }

        [HttpPost, ActionName ("Delete")]
        public async Task<ActionResult> DeletePost (int? id)
        {
            using (DbContext _context = new DbContext (_connectionString))
            {
                var departmentToDelete = await _context.GetConnection ().GetAsync<Department> (id);
                await _context.GetConnection ().DeleteAsync (departmentToDelete);
            }
            return RedirectToAction ("Index");
        }

        private void PopulateInstructorDepartmentList (object selectedInstructor = null)
        {
            IEnumerable<Instructor> instructors = Enumerable.Empty<Instructor> ();
            using (DbContext _context = new DbContext (_connectionString))
            {
                instructors = _context.GetConnection ().GetAll<Instructor> ().OrderBy (s => s.LastName).Select (s => s);
            }
            ViewBag.InstructorId = new SelectList (instructors, "Id", "FullName", selectedInstructor);
        }
    }
}
