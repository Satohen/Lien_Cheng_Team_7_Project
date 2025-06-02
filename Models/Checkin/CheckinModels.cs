using System.ComponentModel.DataAnnotations.Schema;

namespace 第7小組專題.Models.Checkin
{
    public class CheckinModels
    {
        [Column("Id")]
        public int id { get; set; }  // 出勤紀錄編號

        [Column("EmployeeId")]
        public int employeeId { get; set; }  // 員工 ID

        [Column("Date")]
        public DateTime date { get; set; }  // 出勤日期

        [Column("CheckInTime")]
        public DateTime? checkInTime { get; set; }  // 上班打卡時間

        [Column("CheckOutTime")]
        public DateTime? checkOutTime { get; set; }  // 下班打卡時間

        [Column("Note")]
        public string? note { get; set; }  // 備註

        [Column("IsLate")]
        public Boolean? isLate { get; set; }  // 備註

        public string? leaveType { get; set; } //請假類型

        public string? leaveStatus { get; set; } //請假狀態

        public bool isWorkday { get; set; }

    }
}
