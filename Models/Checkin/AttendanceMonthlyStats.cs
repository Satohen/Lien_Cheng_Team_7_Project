namespace 第7小組專題.Models.Checkin
{
    public class AttendanceMonthlyStats
    {
        public int Month { get; set; } // 月份 (1~12)
        public int TotalDays { get; set; } // 工作日總數
        public int PresentDays { get; set; } // 實際出勤天數

    }
}
