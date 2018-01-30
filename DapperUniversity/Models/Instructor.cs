using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DapperUniversity.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        public OfficeAssignment OfficeAssignment { get; set; }
        public IEnumerable<CourseAssignment> CourseAssignments { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => LastName + ", " + FirstName;
    }
}
