using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using MediatR;

namespace DapperUniversity.Features.Courses
{
    public class Index
    {
        public class Query : IRequest<Model>
        {
            public Department SelectedDepartment { get; set; }
        }

        public class Model
        {
            public IEnumerable<Course> Courses { get; set; }
            public Department SelectedDepartment { get; set; }
        }

        public class Handler : AsyncRequestHandler<Query, Model>
        {

            private readonly string _connectionString;

            public Handler(string connectionString)
            {
                _connectionString = connectionString;
            }

            protected override async Task<Model> HandleCore(Query message)
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

                return new Model
                {
                    Courses = courses,
                    SelectedDepartment = message.SelectedDepartment
                };
            }
        }
    }
}
