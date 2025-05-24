using 第7小組專題.Models.Checkin;
using 第7小組專題.Repository.Checkin;

namespace 第7小組專題.Services.Checkin
{
    public class CheckinService
    {
        private readonly CheckinRepository _repo;

        public CheckinService(IConfiguration config)
        {
            _repo = new CheckinRepository(config);
        }

        //  查詢某員工某月份打卡紀錄
        public List<CheckinModels> GetCheckinsByMonth(int employeeId, string month)
        {
            return _repo.GetFullMonthRecords(employeeId, month);
        }
        public object CheckIn(int employeeId)
        {
            var today = DateTime.Today;
            var existing = _repo.GetRecordByDate(employeeId, today);
            var now = DateTime.Now;
            var normalCheckinEnd = new TimeSpan(10, 0, 0); // 10:00 AM 可自己設定幾點遲到
            var isLate = now.TimeOfDay > normalCheckinEnd;

            // 可將 isLate 存入 DB 中的欄位（例如 LateFlag = 1）



            if (existing != null && existing.checkInTime != null)
            {
                return new { message = "您今天已經出勤過囉～", status = "exists" };
            }

            return _repo.InsertCheckIn(employeeId, today, isLate);
        }

        public object CheckOut(int employeeId)
        {
            var today = DateTime.Today;
            var existing = _repo.GetRecordByDate(employeeId, today);

            if (existing == null || existing.checkInTime == null)
            {
                return new { message = "尚未出勤，不能退勤", status = "error" };
            }

            if (existing.checkOutTime != null)
            {
                return new { message = "您今天已經退勤過囉～", status = "exists" };
            }

            return _repo.UpdateCheckOut(employeeId, today);
        }


        public CheckinModels? GetRecordByDate(int employeeId, DateTime date)
        {
            return _repo.GetRecordByDate(employeeId, date);
        }

    }
}
