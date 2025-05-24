namespace 第7小組專題.Models.Checkin
{
    public class CheckinQueryRequest
    {
        public int employeeId { get; set; }   // 查詢的員工 ID
        public string month { get; set; }     // 格式為 yyyy-MM，例如 "2025-05"

    }
}
