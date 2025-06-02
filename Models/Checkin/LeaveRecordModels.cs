namespace 第7小組專題.Models.Checkin
{
    public class LeaveRecordModels
    {
        public string? leaveType { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string? reason { get; set; }
        public string? status { get; set; }
        public string? attachmentPath { get; set; }
    }
}
