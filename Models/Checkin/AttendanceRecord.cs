namespace 第7小組專題.Models.Checkin
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? Note { get; set; }
    }
}
