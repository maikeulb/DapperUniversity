using DapperUniversity.Models;
using Dapper.FluentMap.Mapping;

namespace DapperUniversity.Data
{
    public class OfficeAssignmentMap : EntityMap<OfficeAssignment>
    {
        public OfficeAssignmentMap()
        {
            Map(oa => oa.InstructorId)
                .ToColumn("instructor_id");
            Map(oa => oa.Location)
                .ToColumn("location");
        }
    }
}
