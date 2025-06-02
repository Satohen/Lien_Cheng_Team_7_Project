using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

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
        public IActionResult login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest("帳號或密碼不能空白");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = "SELECT id, password, role FROM accountInformation WHERE userName = @u";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string hashedPassword = reader.GetString(1);
                            string role = reader.GetString(2);

                            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                            if (isPasswordMatch)
                            {
                                HttpContext.Session.SetInt32("id", id);
                                HttpContext.Session.SetString("username", username);
                                HttpContext.Session.SetString("role", role);

                                return Json(new { status = "success", id, username, role });
                            }
                        }

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
                string sql = "INSERT INTO accountInformation (userName, password, name, email, role) VALUES (@u, @p, @n, @e, @r)";

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", hashedPassword); 
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@r", "User");
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

        [HttpGet("list")]
        public IActionResult GetAccountList()
        {
            var role = HttpContext.Session.GetString("role");

            if (role != "Admin")
            {
                return Unauthorized("您沒有權限查看帳號清單");
            }

            List<object> accounts = new List<object>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT userName, name, email, role FROM accountInformation";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new
                            {
                                Username = reader["userName"].ToString(),
                                Name = reader["name"].ToString(),
                                Email = reader["email"].ToString(),
                                Role = reader["role"].ToString()
                            });
                        }
                    }
                }
            }

            return Json(accounts);
        }

        [HttpPost("update-role")]
        public IActionResult UpdateRole(string username, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role))
                return BadRequest("帳號或角色不能空白");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE accountInformation SET role = @r WHERE userName = @u";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@r", role);
                    int rows = cmd.ExecuteNonQuery();
                    return Content(rows > 0 ? "success" : "fail");
                }
            }
        }

        [HttpPost("delete-account")]
        public IActionResult DeleteAccount(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("帳號不能空白");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM accountInformation WHERE userName = @u";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    int rows = cmd.ExecuteNonQuery();
                    return Content(rows > 0 ? "success" : "fail");
                }
            }
        }


    }
}
