using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using MediatR;

namespace DapperUniversity.Features.Courses
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
            public Department SelectedDepartment { get; set; }
        }

        public class Result
        {
            public Department SelectedDepartment { get; set; }
            public IEnumerable<Course> Courses { get; set; }

            public class Course
            {
                public int Id { get; set; }
                public string Title { get; set; }
                public int Credits { get; set; }
                public string DepartmentName { get; set; }
            }
        }

        public class Handler : AsyncRequestHandler<Query, Result>
        {

            private readonly string _connectionString;

            public Handler(string connectionString)
            {
                _connectionString = connectionString;
            }

            protected override  async Task<Result> HandleCore(Query message)
            {
                int? departmentID = message.SelectedDepartment?.Id;

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

                return new Result
                {
                    Courses = courses.Cast<Result.Course>(),
                    SelectedDepartment = message.SelectedDepartment
                };
            }
        }
    }
}
