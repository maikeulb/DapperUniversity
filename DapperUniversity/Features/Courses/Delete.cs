using System.Collections.Generic;
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
    public class Delete
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
                Course courseToDelete = new Course();

                using (DbContext _context = new DbContext(_connectionString))
                {
                    courseToDelete = await GetCourseDepartment(message.Id);
                }
               
                Command command = new Command 
                {
                    course = courseToDelete
                };

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
        }

        public class Command : IRequest
        {
            public Course course;
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
                using (DbContext _context = new DbContext(_connectionString))
                {
                    var _connection = _context.GetConnection();
                    /* await _connection.OpenAsync(); */
                    _connection.Open();
                    var courseToDelete = await _connection.GetAsync<Course>(message.course.Id);
                    await _connection.DeleteAsync(courseToDelete);
                }
            }
        }
    }
}
