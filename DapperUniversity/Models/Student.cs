using Dapper;
using System;
using System.Collections.Generic;

namespace DapperUniversity.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}
