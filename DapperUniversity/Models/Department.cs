using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DapperUniversity.Models
{
    public sealed class Department
    {
        public int Id { get; set; }
        public int? InstructorId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; } 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public Instructor Instructor { get; set; }
        public IEnumerable<Course> Courses { get; set; }
    }
}
