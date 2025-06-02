using Microsoft.Data.SqlClient;
using 第7小組專題.Models.Checkin;

namespace 第7小組專題.Repository.Checkin
{
    public class AttendanceRepository
    {
        private readonly string _connStr;

        public AttendanceRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
        }

        public List<MonthlyAttendanceSummary> FetchMonthlyAttendanceSummary(int year, int employeeId)
        {
            var result = new List<MonthlyAttendanceSummary>();

            using var conn = new SqlConnection(_connStr);
            conn.Open();

            var cmd = new SqlCommand(@"
                WITH Dates AS (
                    SELECT CAST(@StartDate AS DATE) AS Date
                    UNION ALL
                    SELECT DATEADD(DAY, 1, Date)
                    FROM Dates
                    WHERE DATEADD(DAY, 1, Date) <= @EndDate
                )
                SELECT
                    MONTH(D.Date) AS Month,
                    COUNT(DISTINCT CASE WHEN ISNULL(W.IsWorkday, 1) = 1 THEN D.Date END) AS TotalDays,
                    COUNT(DISTINCT CASE WHEN ISNULL(W.IsWorkday, 1) = 1 AND A.Id IS NOT NULL THEN D.Date END) AS PresentDays,
                    COUNT(DISTINCT CASE WHEN ISNULL(W.IsWorkday, 1) = 1 AND L.LeaveType IS NOT NULL THEN D.Date END) AS LeaveDays,
                    COUNT(DISTINCT CASE WHEN ISNULL(W.IsWorkday, 1) = 1 AND L.LeaveType = '事假' THEN D.Date END) AS PersonalLeaveDays,
                    COUNT(DISTINCT CASE WHEN ISNULL(W.IsWorkday, 1) = 1 AND L.LeaveType = '病假' THEN D.Date END) AS SickLeaveDays,
                    COUNT(DISTINCT CASE WHEN ISNULL(W.IsWorkday, 1) = 1 AND L.LeaveType = '特休' THEN D.Date END) AS AnnualLeaveDays
                FROM Dates D
                LEFT JOIN WorkingCalendar W ON W.WorkDate = D.Date
                LEFT JOIN AttendanceRecords A ON A.Date = D.Date AND A.EmployeeId = @EmployeeId
                LEFT JOIN LeaveRequests L ON L.LeaveDate = D.Date AND L.EmployeeId = @EmployeeId
                WHERE YEAR(D.Date) = @Year
                GROUP BY MONTH(D.Date)
                ORDER BY Month
                OPTION (MAXRECURSION 366);
            ", conn);

            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@StartDate", new DateTime(year, 1, 1));
            cmd.Parameters.AddWithValue("@EndDate", new DateTime(year, 12, 31));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new MonthlyAttendanceSummary
                {
                    Month = reader.GetInt32(0),
                    TotalDays = reader.GetInt32(1),
                    PresentDays = reader.GetInt32(2),
                    LeaveDays = reader.GetInt32(3),
                    PersonalLeaveDays = reader.GetInt32(4),
                    SickLeaveDays = reader.GetInt32(5),
                    AnnualLeaveDays = reader.GetInt32(6)
                });
            }

            return result;
        }



    }
}
