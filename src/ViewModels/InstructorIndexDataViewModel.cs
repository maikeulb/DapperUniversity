using System;
using System.Collections.Generic;
using System.Linq;
using DapperUniversity.Models;

namespace DapperUniversity.ViewModels
{
    public class InstructorIndexDataViewModel
    {
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}
