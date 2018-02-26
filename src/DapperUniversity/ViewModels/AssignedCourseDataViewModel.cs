using System;
using System.Collections.Generic;
using System.Linq;

namespace DapperUniversity.ViewModels
{
    public class AssignedCourseDataViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }
    }
}
