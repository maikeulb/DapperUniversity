using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using DapperUniversity.Models;

namespace DapperUniversity.ViewModels
{
    public class CourseEditViewModel
    {
        public int? SelectedItemId { get; set; }
        public SelectList Department { get; set; }
        public Course Course { get; set; }
    }
}
