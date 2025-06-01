namespace 第7小組專題.Models.Checkin
{
    public class MonthlyAttendanceSummary
    {
        public int Month { get; set; }
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int LeaveDays { get; set; }
        public int PersonalLeaveDays { get; set; }
        public int SickLeaveDays { get; set; }
        public int AnnualLeaveDays { get; set; }
        public int AbsentDays => TotalDays - PresentDays - LeaveDays;

    }
}
