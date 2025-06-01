namespace 第7小組專題.Models.Checkin
{
    public class AttendanceSummary
    {
        public string Date { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public bool IsWorkday { get; set; } = true;
    }
}
