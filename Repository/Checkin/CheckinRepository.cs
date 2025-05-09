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

        public List<CheckinModels> GetAll()
        {
            var list = new List<CheckinModels>();
            using var conn = new SqlConnection(_connStr);
            conn.Open();
            var sql = "SELECT Id, EmployeeName, CheckinTime FROM CheckinRecords";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CheckinModels
                {
                    id = (int)reader["id"],
                    employeeName = reader["employeeName"].ToString(),
                    checkinTime = (DateTime)reader["checkinTime"]
                });
            }
            return list;
        }
    }
}
