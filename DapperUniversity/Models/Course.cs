using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DapperUniversity.Models
{
    public class Course
    {
        [Display(Name = "Number")]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public int DepartmentId { get; set; }

        public Department Department { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
        public IEnumerable<CourseAssignment> CourseAssignments { get; set; }
    }
}
