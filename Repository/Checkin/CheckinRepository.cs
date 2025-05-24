using Microsoft.Data.SqlClient;
using 第7小組專題.Models.Checkin;

namespace 第7小組專題.Repository.Checkin
{
    public class CheckinRepository
    {
        private readonly string _connStr;

        //從config 檔案拿到連線資訊
        public CheckinRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")!;
        }

        // 取得用戶某月份紀錄
        public List<CheckinModels> GetFullMonthRecords(int employeeId, string month)
        {
            var result = new List<CheckinModels>();
            var firstDay = DateTime.Parse($"{month}-01");
            var lastDay = firstDay.Month == DateTime.Today.Month && firstDay.Year == DateTime.Today.Year
                ? DateTime.Today
                : firstDay.AddMonths(1).AddDays(-1);

            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                WITH Dates AS (
                    SELECT @FirstDay AS Date
                    UNION ALL
                    SELECT DATEADD(DAY, 1, Date)
                    FROM Dates
                    WHERE DATEADD(DAY, 1, Date) <= @LastDay
                )
                SELECT 
                    D.Date,
                    A.Id,
                    A.EmployeeId,
                    A.CheckInTime,
                    A.CheckOutTime,
                    A.IsLate,
                    A.Note,
                    L.LeaveType,
                    L.Status AS LeaveStatus
                FROM Dates D
                LEFT JOIN AttendanceRecords A
                    ON A.Date = D.Date AND A.EmployeeId = @EmployeeId
                LEFT JOIN LeaveRequests L
                    ON L.LeaveDate = D.Date AND L.EmployeeId = @EmployeeId
                OPTION (MAXRECURSION 31)", conn);

                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@FirstDay", firstDay);
                cmd.Parameters.AddWithValue("@LastDay", lastDay);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new CheckinModels
                    {
                        id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                        employeeId = employeeId,
                        date = (DateTime)reader["Date"],
                        checkInTime = reader["CheckInTime"] as DateTime?,
                        checkOutTime = reader["CheckOutTime"] as DateTime?,
                        isLate = reader["IsLate"] != DBNull.Value && (bool)reader["IsLate"],
                        note = reader["Note"]?.ToString(),
                        leaveType = reader["LeaveType"]?.ToString(),
                        leaveStatus = reader["LeaveStatus"]?.ToString()
                    });
                }
            }

            return result;
        }



        public object InsertCheckIn(int employeeId, DateTime date, bool isLate)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            var cmd = new SqlCommand(@"
        INSERT INTO AttendanceRecords (EmployeeId, Date, CheckInTime, IsLate)
        VALUES (@EmployeeId, @Date, @CheckInTime, @IsLate)", conn);

            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@CheckInTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@IsLate", isLate ? 1 : 0); // BIT 欄位

            cmd.ExecuteNonQuery();
            return new { message = isLate ? "已簽到（遲到）" : "簽到成功！", status = "checkin" };
        }

        public object UpdateCheckOut(int employeeId, DateTime date)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();
            var cmd = new SqlCommand(@"
        UPDATE AttendanceRecords
        SET CheckOutTime = @CheckOutTime
        WHERE EmployeeId = @EmployeeId AND Date = @Date", conn);
            cmd.Parameters.AddWithValue("@CheckOutTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.ExecuteNonQuery();
            return new { message = "退勤成功！", status = "checkout" };
        }

        //檢查是否已簽到
        public CheckinModels? GetRecordByDate(int employeeId, DateTime date)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();
            var cmd = new SqlCommand(@"
        SELECT * FROM AttendanceRecords
        WHERE EmployeeId = @EmployeeId AND Date = @Date", conn);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@Date", date);

            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new CheckinModels
                {
                    id = (int)reader["Id"],
                    employeeId = (int)reader["EmployeeId"],
                    date = (DateTime)reader["Date"],
                    checkInTime = reader["CheckInTime"] as DateTime?,
                    checkOutTime = reader["CheckOutTime"] as DateTime?,
                    note = reader["Note"]?.ToString()
                };
            }
            return null;
        }

    }
}
