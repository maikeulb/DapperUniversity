using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;

namespace DapperUniversity.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly string _connectionString;

        public DepartmentsController()
        {
            _connectionString = "Server=172.17.0.2;Port=5432;Database=DapperUniversity;User ID=postgres;Password=P@ssw0rd!;";
        }

        public async Task<ActionResult> Index()
        {

            IEnumerable<Course> courses = Enumerable.Empty<Course>();
            IEnumerable<Department> departments = Enumerable.Empty<Department>();

            string query = @"SELECT d.*, i.*
                             FROM departments d
                               INNER JOIN instructors i 
                               ON i.id = d.instructor_id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                var _connection = _context.GetConnection();
                _connection.Open();
                courses = await _connection.GetAllAsync<Course>();
                departments =  await _connection.QueryAsync<Department, Instructor, Department>(query,
                ((department, instructor) =>
                {
                    department.Instructor = instructor;
                    return department;
                }));
            }

            return View(departments.ToList());
        }

        public async Task<ActionResult> Details(int? id)
        {
            string query = @"SELECT d.*, i.last_name, i.first_name 
                             FROM departments d
                             INNER JOIN instructors i 
                               ON i.id = d.instructor_id
                               WHERE d.id = @id";

            IEnumerable<Department> departments = Enumerable.Empty<Department>();
            using (DbContext _context = new DbContext(_connectionString))
            {
                departments = await _context.GetConnection().QueryAsync<Department, Instructor, Department>(query,
                ((department, instructor) =>
                {
                    department.Instructor = instructor;
                    return department;
                }));
            }

            return View(departments.FirstOrDefault());
        }

        public ActionResult Create()
        {
            PopulateInstructorDepartmentList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Bind("Name, Budget, InstructorId, StartDate")]Department department)
        {
            string command = @"INSERT INTO departments (name, budget, instructor_id, start_date) 
                               VALUES(@Name, @Budget, @InstructorId, @StartDate)";

            using (DbContext _context = new DbContext(_connectionString))
            {
                await _context.GetConnection().ExecuteAsync(command, department);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            Department department = new Department();

            using (DbContext _context = new DbContext(_connectionString))
            {
                department = await _context.GetConnection().GetAsync<Department>(id);
            }

            PopulateInstructorDepartmentList(department.Id);

            return View(department);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<ActionResult> EditPost (int id)
        {
            Department department = new Department();

            string command = @"UPDATE departments 
                               SET instructor_id = @InstructorId, 
                                   name = @Name,
                                   budget = @Budget,
                                   start_date = @StartDate
                               WHERE id = @Id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                department  = await _context.GetConnection().GetAsync<Department>(id);
                if (await TryUpdateModelAsync<Department>(
                    department,
                    "",
                    s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorId))
                {
                    await _context.GetConnection().ExecuteAsync(command, department);
                }
                return RedirectToAction("Index");
            }
            return View(department); 
        }

        [HttpGet]
        public async Task<ActionResult> Delete (int? id)
        {
            Department department = new Department();

            using (DbContext _context = new DbContext(_connectionString))
            {
                department = await _context.GetConnection().GetAsync<Department>(id);
            }

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeletePost (int? id)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                var departmentToDelete = await _context.GetConnection().GetAsync<Department>(id);
                await _context.GetConnection().DeleteAsync(departmentToDelete);
            }
            return RedirectToAction("Index"); 
        }

        private void PopulateInstructorDepartmentList(object selectedInstructor = null)
        {
            IEnumerable<Instructor> instructors = Enumerable.Empty<Instructor>();
            using (DbContext _context = new DbContext(_connectionString))
            {
                instructors = _context.GetConnection().GetAll<Instructor>().OrderBy(s => s.LastName).Select(s => s);
            }
            ViewBag.InstructorId = new SelectList(instructors, "Id", "FullName", selectedInstructor);
        }
    }
}
