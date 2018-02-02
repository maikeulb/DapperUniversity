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

        public CoursesController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<IEnumerable<Course>> Index(int? id, int? courseId)
        {
            IEnumerable<Course> courses = Enumerable.Empty<Course>(); 

            var query = @"SELECT c.*, d.name 
                          FROM course AS c 
                            INNER JOIN department AS d
                            ON d.id = c.department_id;";

            using (DbContext _context = new DbContext(_connectionString))
            {
                courses  = await _context.GetConnection().QueryAsync<Course, Department, Course> (query,
                    (courseItem, deparment) =>
                    {
                        courseItem.Department = deparment;
                        return courseItem;
                    },
                        splitOn: "id");
            }
            return courses;

        }

        [HttpGet]
        public async Task<Course> Details(int? id)
        {
            return await GetCourseDepartment(id);
        }

        [HttpGet]
        public CourseEditViewModel Create()
        {
            CourseEditViewModel  model = new CourseEditViewModel();

            PopulateDropDownList(model);

            return model;
        }

        [HttpPost]
        public async Task Create ([Bind("Id, Title, Credits, DepartmentId")]Course course)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                await _context.GetConnection().InsertAsync(course);
            }
            return; 
        }

        [HttpGet]
        public async Task<CourseEditViewModel> Edit(int? id)
        {

            var model = new CourseEditViewModel();
            using (DbContext _context = new DbContext(_connectionString))
            {
                model.Course = await _context.GetConnection().GetAsync<Course>(id);
            }
            PopulateDropDownList(model);
            return model;
        }

        [HttpPost]
        public async Task EditPost(int? id)
        {  
            CourseEditViewModel model = new CourseEditViewModel();
            using (DbContext _context = new DbContext(_connectionString))
            {
               var _connection = _context.GetConnection();
               _connection.Open();
               model.Course = await _connection.GetAsync<Course>(id);
               await _connection.UpdateAsync(model.Course);
            }
        }

        [HttpGet]
        public async Task<Course> Delete(int? id)
        {
            Course course = new Course();

            using (DbContext _context = new DbContext(_connectionString))
            {
                course = await GetCourseDepartment(id);
            }
            return course;
        }

        [HttpPost]
        public async Task Delete(int id)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                var _connection = _context.GetConnection();
                /* await _connection.OpenAsync(); */
                _connection.Open();
                var courseToDelete = await _connection.GetAsync<Course>(id);
                await _connection.DeleteAsync(courseToDelete);
            }
            return; 
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

            var query = @"SELECT c.*, d.name FROM course c
                      INNER JOIN department d ON d.id = c.departmentId
                      WHERE c.id = @id";

            using (DbContext _context = new DbContext(_connectionString))
            {
                courses = await _context.GetConnection().QueryAsync<Course, Department, Course>(query,
                    ((course, department) =>
                    {
                        course.Department = department;
                        return course;
                    }),
                    new { id },
                    splitOn: "id");
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
