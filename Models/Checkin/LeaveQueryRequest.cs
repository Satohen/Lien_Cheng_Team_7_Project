namespace 第7小組專題.Models.Checkin
{
    public class LeaveQueryRequest
    {
        public int employeeId { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
