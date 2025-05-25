using Microsoft.Data.SqlClient;
using 第7小組專題.Models.Checkin;

namespace 第7小組專題.Repository.Checkin
{
    public class LeaveRepository
    {
        private readonly string _connStr;

        public LeaveRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
        }

        public bool InsertLeaveRequest(LeaveApplyRequest request, string? attachmentPath)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            var cmd = new SqlCommand(@"
        INSERT INTO LeaveRequests (EmployeeId, LeaveType, LeaveDate, EndDate, Reason, Status, AttachmentPath)
        VALUES (@EmployeeId, @LeaveType, @FromDate, @ToDate, @Reason, 'Pending', @AttachmentPath)", conn);

            cmd.Parameters.AddWithValue("@EmployeeId", request.employeeId);
            cmd.Parameters.AddWithValue("@LeaveType", request.leaveType);
            cmd.Parameters.AddWithValue("@FromDate", request.fromDate);
            cmd.Parameters.AddWithValue("@ToDate", request.toDate);
            cmd.Parameters.AddWithValue("@Reason", (object?)request.reason ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AttachmentPath", (object?)attachmentPath ?? DBNull.Value);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool HasOverlappingLeave(int employeeId, DateTime fromDate, DateTime toDate)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            var cmd = new SqlCommand(@"
            SELECT COUNT(1)
            FROM LeaveRequests
            WHERE EmployeeId = @EmployeeId
            AND Status != 'Rejected'
            AND (
                    (LeaveDate BETWEEN @From AND @To)
                 OR (EndDate BETWEEN @From AND @To)
                 OR (@From BETWEEN LeaveDate AND EndDate)
                 OR (@To BETWEEN LeaveDate AND EndDate)
          )", conn);

            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@From", fromDate);
            cmd.Parameters.AddWithValue("@To", toDate);

            return (int)cmd.ExecuteScalar() > 0;
        }


        public (List<LeaveRecordModels>, int) GetPagedRecords(LeaveQueryRequest request)
        {
            var result = new List<LeaveRecordModels>();
            int total = 0;

            using var conn = new SqlConnection(_connStr);
            conn.Open();

            // 取得總筆數
            var countCmd = new SqlCommand(@"
        SELECT COUNT(*) FROM LeaveRequests
        WHERE EmployeeId = @EmployeeId", conn);
            countCmd.Parameters.AddWithValue("@EmployeeId", request.employeeId);
            total = (int)countCmd.ExecuteScalar();

            // 取得分頁資料
            var cmd = new SqlCommand(@"
        SELECT * FROM LeaveRequests
        WHERE EmployeeId = @EmployeeId
        ORDER BY LeaveDate DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn);

            cmd.Parameters.AddWithValue("@EmployeeId", request.employeeId);
            cmd.Parameters.AddWithValue("@Offset", (request.page - 1) * request.pageSize);
            cmd.Parameters.AddWithValue("@PageSize", request.pageSize);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new LeaveRecordModels
                {
                    fromDate = (DateTime)reader["LeaveDate"],
                    toDate = (DateTime)reader["EndDate"],
                    leaveType = reader["LeaveType"].ToString(),
                    reason = reader["Reason"]?.ToString(),
                    status = reader["Status"].ToString(),
                    attachmentPath = reader["AttachmentPath"]?.ToString()
                });
            }

            return (result, total);
        }

}
}
