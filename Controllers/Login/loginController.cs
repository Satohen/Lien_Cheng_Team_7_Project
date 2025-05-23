using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace 第7小組專題.Controllers.Login
{
    [Route("login")]
    public class loginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=account;User ID=don1;Password=Qw669668;Trusted_Connection=True";
        [HttpPost("login")]
        public IActionResult login(string username,string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest("帳號或密碼不能空白");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM accountInformation WHERE userName = @u AND password = @p";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password); // ⚠️ 實務應改用雜湊密碼驗證

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("fail");
                    }
                }
            }
        }
        [HttpPost("signup")]
        public IActionResult Signup(string username, string password, string name, string email)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                return BadRequest("欄位不能空白");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO accountInformation (userName, password,name,email) VALUES (@u, @p,@n,@e)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password); // ⚠️ 實務應改用雜湊密碼驗證
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@e", email);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("fail");
                    }
                }
            }
        }
    }
}
