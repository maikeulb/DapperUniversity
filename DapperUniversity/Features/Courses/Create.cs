using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using MediatR;

namespace DapperUniversityCore.Features.Courses
{
    public class Create
    {
        public class Command : IRequest
        {
            [IgnoreMap]
            public int Number { get; set; }
            public string Title { get; set; }
            public int Credits { get; set; }
            public Department Department { get; set; }
        }

        public class Handler : RequestHandler<Command>
        {
            private readonly string _connectionString;

            public Handler(string connectionString)
            {
                _connectionString = connectionString;
            }

            protected override void HandleCore(Command message)
            {
                var course = Mapper.Map<Command, Course>(message);
                course.Id = message.Number;

                using (DbContext _context = new DbContext(_connectionString))
                {
                    _context.GetConnection().Insert(course);
                }
                return;
            }
        }
    }
}
