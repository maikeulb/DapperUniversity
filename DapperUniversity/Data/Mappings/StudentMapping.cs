using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class StudentMap : EntityMap<Student>
    {
        public StudentMap()
        {
            Map(p => p.StudentId)
                .ToColumn("id");
            Map(p => p.LastName)
                .ToColumn("last_name");
            Map(p => p.FirstName)
                .ToColumn("first_name");
            Map(p => p.EnrollmentDate)
                .ToColumn("enrollment_date");
        }
    }
}
