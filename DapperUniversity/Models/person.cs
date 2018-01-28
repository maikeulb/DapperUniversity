using Dapper;

namespace DapperUniversity.Models
{
    public class person
    {
        public int id { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
    }
}
