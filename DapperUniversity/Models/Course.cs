using System.Collections.Generic;

namespace DapperUniversity.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}
