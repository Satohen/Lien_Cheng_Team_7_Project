using 第7小組專題.Models.Checkin;
using 第7小組專題.Repository.Checkin;

namespace 第7小組專題.Services.Checkin

{
    public class AttendanceService
    {
        private readonly AttendanceRepository _repo;

        public AttendanceService(IConfiguration config)
        {
            _repo = new AttendanceRepository(config);
        }

        public List<MonthlyAttendanceSummary> GetMonthlySummary(int year, string employeeId)
        {
            return _repo.FetchMonthlyAttendanceSummary(year, employeeId);
        }
    }

}
