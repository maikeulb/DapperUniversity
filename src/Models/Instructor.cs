using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace DapperUniversity.Models
{
    public class Instructor
    {
        private readonly List<Course> _courses = new List<Course> ();

        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        public OfficeAssignment OfficeAssignment { get; set; }
        public ICollection<CourseAssignment> CourseAssignments { get; set; }
        public ICollection<Course> Courses => _courses.AsReadOnly ();

        [Display(Name = "Full Name")]
        public string FullName => LastName + ", " + FirstName;

        public void AddCourse (Course course) => _courses.Add (course);

        public void UpdateInstructorCourses(string[] selectedCourses, IEnumerable<Course> courses)
        {
            if (selectedCourses == null)
            {
                CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedCourseHash = new HashSet<string>(selectedCourses);
            var instructorCourseHash = new HashSet<int>(CourseAssignments.Select(c => c.CourseId));

            foreach (var course in courses)
            {
                if (selectedCourseHash.Contains(course.Id.ToString()))
                {
                    if (!instructorCourseHash.Contains(course.Id))
                    {
                        CourseAssignments.Add(new CourseAssignment { Course = course, Instructor = this });
                    }
                }
                else
                {
                    if (instructorCourseHash.Contains(course.Id))
                    {
                        var toRemove = CourseAssignments.Single(ci => ci.CourseId == course.Id);
                        CourseAssignments.Remove(toRemove);
                    }
                }
            }
        }   }
}
