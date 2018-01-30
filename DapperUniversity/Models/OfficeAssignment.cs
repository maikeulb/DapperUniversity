using System.ComponentModel.DataAnnotations;

namespace DapperUniversity.Models
{
    public class OfficeAssignment
    {
        public int InstructorId { get; set; }
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        public Instructor Instructor { get; set; }
    }

}
