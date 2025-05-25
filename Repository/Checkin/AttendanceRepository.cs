using Microsoft.Data.SqlClient;

namespace 第7小組專題.Repository.Checkin
{
    public class AttendanceRepository
    {
        private readonly string _connStr;

        public AttendanceRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
        }

        public Dictionary<int, int> FetchMonthlyAttendanceDays(int year, string employeeId)
        {
            var result = new Dictionary<int, int>();

            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var sql = @"
                           SELECT MONTH(CheckInTime) AS Month,
                           COUNT(DISTINCT CAST(CheckInTime AS DATE)) AS Days
                            FROM AttendanceRecords
                            WHERE YEAR(CheckInTime) = @Year AND EmployeeId = @EmployeeId
                            GROUP BY MONTH(CheckInTime)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);


                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int month = reader.GetInt32(0);  // 1~12
                                int days = reader.GetInt32(1);
                                result[month] = days;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("SQL 錯誤：" + ex.Message);
                        throw; // 可以先 throw 回 controller 看錯誤訊息
                    }

                }
            }

            return result;
        }
    }
}
