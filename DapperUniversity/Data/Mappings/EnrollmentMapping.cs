using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class EnrollmentMap : EntityMap<Enrollment>
    {
        public EnrollmentMap()
        {
            Map(p => p.EnrollmentId)
                .ToColumn("enrollment_id");
            Map(p => p.CourseId)
                .ToColumn("course_id");
            Map(p => p.StudentId)
                .ToColumn("student_id");
            Map(p => p.Grade)
                .ToColumn("grade");
        }
    }
}
