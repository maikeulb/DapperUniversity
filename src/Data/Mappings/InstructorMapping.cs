using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class InstructorMap : EntityMap<Instructor>
    {
        public InstructorMap()
        {
            Map(i => i.Id)
                .ToColumn("id");
            Map(i => i.FirstName)
                .ToColumn("first_name");
            Map(i => i.LastName)
                .ToColumn("last_name");
            Map(i => i.HireDate)
                .ToColumn("hire_date");
            Map(i => i.OfficeAssignment)
                .Ignore();
        }
    }
}
