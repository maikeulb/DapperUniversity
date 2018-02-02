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

        public DepartmentsController(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Department>> Index()
        {

            IEnumerable<Course> courses = Enumerable.Empty<Course>();
            IEnumerable<Department> departments = Enumerable.Empty<Department>();

            var query = @"SELECT d.*, i.first_name, i.last_name 
                      FROM department d
                        INNER JOIN instructor i 
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
                }), splitOn: "id");
            }

            return departments.ToList();
        }

        public async Task<Department> Details(int? id)
        {
            var query = @"SELECT d.*, i.last_name, i.first_name 
                          FROM department d
                          INNER JOIN instructor i 
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
                }), new {id},
                splitOn: "id");
            }

            return departments.FirstOrDefault();
        }

        [HttpGet]
        public void Create()
        {
            return;
        }


        [HttpPost]
        public async Task Create([Bind("Name, Budget, InstructorId, StartDate")]Department department)
        {
            using (DbContext _context = new DbContext(_connectionString))
            {
                 await _context.GetConnection().InsertAsync(department);
            }
            return; 
        }

        [HttpGet]
        public async Task<Department> Edit(int? id)
        {
            Department department = new Department();

            using (DbContext _context = new DbContext(_connectionString))
            {
                department = await _context.GetConnection().GetAsync<Department>(id);
            }

            PopulateInstructorDepartmentList(department.Id);

            return department;
        }

        [HttpPost]
        public void EditPost (int? id)
        {
            throw new NotImplementedException(); 
        }


        [HttpGet]
        public void Delete(int id)
        {
            return;
        }

        [HttpPost]
        public void DeletePost(int id)
        {
            throw new NotImplementedException(); 
        }

        private void PopulateInstructorDepartmentList(object selectedInstructor = null)
        {
            IEnumerable<Instructor> instructors = Enumerable.Empty<Instructor>();
            using (DbContext _context = new DbContext(_connectionString))
            {
                instructors = _context.GetConnection().GetAll<Instructor>().OrderBy(s => s.LastName).Select(s => s);
            }
            ViewBag.InstructorId = new SelectList(instructors, "InstructorId", "FullName", selectedInstructor);
        }
    }
}
