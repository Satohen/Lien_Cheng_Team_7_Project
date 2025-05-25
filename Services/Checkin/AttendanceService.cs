using 第7小組專題.Repository.Checkin;

namespace 第7小組專題.Services.Checkin
{
    public class AttendanceService
    {
        private readonly AttendanceRepository _repo;

        public AttendanceService(AttendanceRepository repo)
        {
            _repo = repo;
        }

        public List<int> GetMonthlyAttendanceDays(int year, string employeeId)
        {
            var data = _repo.FetchMonthlyAttendanceDays(year, employeeId);
            var result = new List<int>();

            // 確保回傳長度為 12（對應 1~12 月）
            for (int month = 1; month <= 12; month++)
            {
                result.Add(data.ContainsKey(month) ? data[month] : 0);
            }

            return result;
        }
    }
}
