using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class StudentMap : EntityMap<Student>
    {
        public StudentMap()
        {
            Map(s => s.Id)
                .ToColumn("id");
            Map(s => s.FirstName)
                .ToColumn("first_name");
            Map(s => s.LastName)
                .ToColumn("last_name");
            Map(s => s.EnrollmentDate)
                .ToColumn("enrollment_date");
        }
    }
}
