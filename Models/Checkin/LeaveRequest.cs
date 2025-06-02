namespace 第7小組專題.Models.Checkin
{
    public class LeaveRequest
    {
        public int id { get; set; }
        public int employeeId { get; set; }
        public DateTime leaveDate { get; set; }
        public string leaveType { get; set; }  // 病假 / 事假 / 特休等
        public string? reason { get; set; }
        public string status { get; set; }     // 送出 / 核准 / 駁回
        public DateTime createdAt { get; set; }
    }
}
