using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class CourseMap : EntityMap<Course>
    {
        public CourseMap()
        {
            Map(p => p.CourseId)
                .ToColumn("course_id");
            Map(p => p.Title)
                .ToColumn("title");
            Map(p => p.Credits)
                .ToColumn("credit");
        }
    }
}
