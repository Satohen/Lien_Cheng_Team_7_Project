namespace 第7小組專題.Models.Checkin
{
    public class LeaveApplyRequest
    {
        public int employeeId { get; set; }
        public string leaveType { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string? reason { get; set; }

    }
}
