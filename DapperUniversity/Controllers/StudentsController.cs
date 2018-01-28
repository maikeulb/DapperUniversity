using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DapperUniversity.Models;
using DapperUniversity.Data;
using Dapper;

namespace DapperUniversity.Controllers
{
    public class StudentsController : Controller
    {
        public const string DatabaseConnectionString = "host=172.17.0.2;port=5432;username=postgres;password=P@ssw0rd!;database=DapperUniversity;";

        private readonly DbContext _context = new DbContext(DatabaseConnectionString);

        [HttpGet]
        public async Task<IEnumerable<person>> Get()
        {
            return await _context.GetConnection().GetListAsync<person>();
        }
   }
}
