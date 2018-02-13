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
using MediatR;

namespace DapperUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private readonly string _connectionString;

        public CoursesController()
        {
            _connectionString = "Server=172.17.0.2;Port=5432;Database=DapperUniversity;User ID=postgres;Password=P@ssw0rd!;";
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
            var courses =  await GetCourseDepartment(id);
            return View(courses);
        }

        [HttpGet]
        public ActionResult Create()
        {
            CourseEditViewModel model = new CourseEditViewModel();

            PopulateDropDownList(model);

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
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            var model = new CourseEditViewModel();
            using (DbContext _context = new DbContext(_connectionString))
            {
                model.Course = await _context.GetConnection().GetAsync<Course>(id);
            }
            PopulateDropDownList(model);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int id)
        {  
            string command = @"UPDATE courses
                               SET id = @Id,
                                   title = @Title,
                                   credits = @Credits,
                                   department_id = @DepartmentId
                               WHERE id = @Id";

            CourseEditViewModel model = new CourseEditViewModel();

            using (DbContext _context = new DbContext(_connectionString))
            {
               var _connection = _context.GetConnection();
               _connection.Open();
               model.Course = await _connection.GetAsync<Course>(id);
               model.Course.Id = id;
               await _connection.ExecuteAsync(command, new {model.Course.Id, model.Course.Title, model.Course.Credits, model.Course.DepartmentId});
               return RedirectToAction("Index");
            }
            PopulateDropDownList(model);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            Course course = new Course();

            using (DbContext _context = new DbContext(_connectionString))
            {
                course = await GetCourseDepartment(id);
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                var _connection = _context.GetConnection();
                _connection.Open();
                var courseToDelete = await _connection.GetAsync<Course>(id);
                await _connection.DeleteAsync(courseToDelete);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        private void PopulateDropDownList(CourseEditViewModel model)
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

namespace DapperUniversity.Models
{
    public class CourseEditViewModel
    {
        public int? SelectedItemId { get; set; }
        public SelectList Department { get; set; }
        public Course Course { get; set; }
    }

}
