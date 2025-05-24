namespace 第7小組專題.Models.Checkin
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime LeaveDate { get; set; }
        public string LeaveType { get; set; }  // 病假 / 事假 / 特休等
        public string? Reason { get; set; }
        public string Status { get; set; }     // 送出 / 核准 / 駁回
        public DateTime CreatedAt { get; set; }
    }
}
