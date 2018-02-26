using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DapperUniversity.ViewModels
{
    public class EnrollmentDateGroupViewModel
    {
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }
        public int StudentCount { get; set; }
    }
}
