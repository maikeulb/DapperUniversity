using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DapperUniversity.Models
{
    public class Course
    {
        [Display(Name = "Number")]
        public int CourseId { get; set; }
        public int DepartmentId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public Department Department { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
        public IEnumerable<CourseAassignment> CourseAssignments { get; set; }
    }
}
