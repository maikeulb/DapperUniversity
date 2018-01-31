using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class EnrollmentMap : EntityMap<Enrollment>
    {
        public EnrollmentMap()
        {
            Map(e => e.Id)
                .ToColumn("id");
            Map(e => e.CourseId)
                .ToColumn("course_id");
            Map(e => e.StudentId)
                .ToColumn("student_id");
            Map(e => e.Grade)
                .ToColumn("grade");
        }
    }
}
