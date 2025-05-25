using System.Text.Json.Serialization;

namespace 第7小組專題.Models.Checkin
{
    public class AttendanceRequest
    {
        public int year { get; set; }

        public string employeeId { get; set; }
    }
}
