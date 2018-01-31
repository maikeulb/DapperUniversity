using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class CourseMap : EntityMap<Course>
    {
        public CourseMap()
        {
            Map(c => c.Id)
                .ToColumn("id");
            Map(c => c.Title)
                .ToColumn("title");
            Map(c => c.Credits)
                .ToColumn("credits");
        }
    }
}
