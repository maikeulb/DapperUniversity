using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class CourseAssignmentMap : EntityMap<CourseAssignment>
    {
        public CourseAssignmentMap()
        {
            Map(ca => ca.InstructorId)
                .ToColumn("instructor_id");
            Map(ca => ca.CourseId)
                .ToColumn("course_id");
        }
    }
}

