using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using MediatR;

namespace DapperUniversity.Features.Courses
{
    public class Edit
    {
        public class Query : IRequest<Command>
        {
            public int? Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class QueryHandler : AsyncRequestHandler<Query, Command>
        {
            private readonly string _connectionString;

            public QueryHandler(string connectionString)
            {
                _connectionString = connectionString;
            }

            protected async override Task<Command> HandleCore(Query message)
            {
                Command command = new Command();

                using (DbContext _context = new DbContext(_connectionString))
                {
                    command.Course = await GetCourseDepartment(message.Id);
                }

                PopulateDropDownList(command);
                return command;
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


            private void PopulateDropDownList(Command model)
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
        }

        public class Command : IRequest
        {
            public int? SelectedItemId { get; set; }
            public SelectList Department { get; set; }
            public Course Course { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(m => m.Course.Title).NotNull().Length(3, 50);
                RuleFor(m => m.Course.Credits).NotNull().InclusiveBetween(0, 5);
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly string _connectionString;

            public CommandHandler(string connectionString)
            {
                _connectionString = connectionString;
            }

            protected override async Task HandleCore(Command message)
            {  
                Command command = new Command();
                using (DbContext _context = new DbContext(_connectionString))
                {
                   var _connection = _context.GetConnection();
                   _connection.Open();
                   command.Course = await _connection.GetAsync<Course>(message.Course.Id);
                   await _connection.UpdateAsync(command.Course);
                }
            }
        }

    }
}
