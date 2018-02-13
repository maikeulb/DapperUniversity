using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class DepartmentMap : EntityMap<Department>
    {
        public DepartmentMap()
        {
            Map(d => d.Id)
                .ToColumn("id");
            Map(d => d.InstructorId)
                .ToColumn("instructor_id");
            Map(d => d.Name)
                .ToColumn("name");
            Map(d => d.Budget)
                .ToColumn("budget");
            Map(d => d.StartDate)
                .ToColumn("start_date");
        }
    }
}
