using System.Text;
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

        public byte[] GenerateAttendanceCsv(int employeeId, string month)
        {
            var records = GetCheckinsByMonth(employeeId, month);
            var sb = new StringBuilder();

            // 加上 BOM：\uFEFF
            sb.Append("\uFEFF");  //  Excel 識別 UTF-8 編碼關鍵

            sb.AppendLine("日期,出勤時間,退勤時間,狀態");

            foreach (var r in records)
            {
                var date = r.date.ToString("yyyy-MM-dd");
                var checkin = r.checkInTime?.ToString("HH:mm") ?? "";
                var checkout = r.checkOutTime?.ToString("HH:mm") ?? "";
                string status;
                if (!r.isWorkday) status = "假日";
                else if (string.IsNullOrEmpty(checkin) && string.IsNullOrEmpty(r.leaveType)) status = "曠班";
                else if (!string.IsNullOrEmpty(r.leaveType)) status = "請假";
                else status = "正常";

                sb.AppendLine($"{date},{checkin},{checkout},{status}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }




    }
}
