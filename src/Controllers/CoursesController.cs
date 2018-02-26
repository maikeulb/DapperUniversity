using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using DapperUniversity.Models;
using DapperUniversity.ViewModels;
using DapperUniversity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DapperUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public CoursesController(ILogger<CoursesController> logger,
                IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString ("DapperUniversity");
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Course> courses = Enumerable.Empty<Course>(); 

            var query = @"SELECT c.*, d.*
                          FROM courses AS c 
                            INNER JOIN departments AS d
                            ON d.id = c.department_id;";

            using (DbContext _context = new DbContext(_connectionString))
            {
                courses  = await _context.GetConnection().QueryAsync<Course, Department, Course> (query,
                    (courseItem, deparment) =>
                    {
                        courseItem.Department = deparment;
                        return courseItem;
                    });
            }

            return View(courses);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var course =  await GetCourseDepartment(id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpGet]
        public ActionResult Create()
        {
            CourseEditViewModel model = new CourseEditViewModel();

            PopulateDepartmentsDropDownList(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create ([Bind("Id, Title, Credits, DepartmentId")]Course course)
        {
            string command = @"INSERT INTO courses (id, title, credits, department_id) 
                               VALUES(@Id, @Title, @Credits, @DepartmentId)";

            using (DbContext _context = new DbContext(_connectionString))
            {
                await _context.GetConnection().ExecuteAsync(command, course);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var model = new CourseEditViewModel();

            string query  = @"SELECT * 
                              FROM courses
                                WHERE id = @id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                model.Course  = await _context.GetConnection().QueryFirstAsync<Course> (query, new {id});
            }
            PopulateDepartmentsDropDownList(model);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id)
        {  
            if (id == null)
            {
                return NotFound();
            }

            CourseEditViewModel model = new CourseEditViewModel();
 
            Course courseToUpdate = new Course();

            string query  = @"SELECT * 
                              FROM courses
                                WHERE id = @id";

            string command = @"UPDATE courses
                               SET title = @Title,
                                   credits = @Credits,
                                   department_id = @DepartmentId
                               WHERE id = @Id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                courseToUpdate = await _context.GetConnection().QueryFirstAsync<Course> (query, new {id});

                if (await TryUpdateModelAsync<Course>(courseToUpdate, "",
                    s => s.Title, s => s.Credits, s => s.DepartmentId))
               {
                    await _context.GetConnection().ExecuteAsync(command, courseToUpdate);
                    return RedirectToAction("Index");
                }
            }
            model.Course = courseToUpdate;
            PopulateDepartmentsDropDownList(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            Course course = new Course();

            using (DbContext _context = new DbContext(_connectionString))
            {
                course = await GetCourseDepartment(id);
            }

            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                var _connection = _context.GetConnection();
                _connection.Open();
                var courseToDelete = await _connection.GetAsync<Course>(id);
                await _connection.DeleteAsync(courseToDelete);
            }
            return RedirectToAction("Index");
        }

        private void PopulateDepartmentsDropDownList(CourseEditViewModel model)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                model.Department = model.Course == null
                    ? model.Department =
                        new SelectList(_context.GetConnection()
                                .GetAll<Department>()
                                .OrderBy(s => s.Name)
                                .Select(s => s), "Id","Name")
                    : model.Department =
                        new SelectList(_context.GetConnection()
                                .GetAll<Department>(), "Id", "Name", model.SelectedItemId = model.Course.DepartmentId);
            }
        }

        private async Task<Course> GetCourseDepartment(int? id)
        {
            IEnumerable<Course> courses = Enumerable.Empty<Course>();

            string query = @"SELECT c.*, d.*
                             FROM courses c
                               INNER JOIN departments d 
                               ON d.id = c.department_id
                               WHERE c.id = @id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                courses = await _context.GetConnection().QueryAsync<Course, Department, Course>(query,
                    ((course, department) =>
                    {
                        course.Department = department;
                        return course;
                    }),
                    new { id });
            }
            return courses.First();
        }

    }
}
