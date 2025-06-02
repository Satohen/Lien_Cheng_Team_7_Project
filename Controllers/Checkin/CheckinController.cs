using Microsoft.AspNetCore.Mvc;
using 第7小組專題.Models.Checkin;
using 第7小組專題.Services.Checkin;

namespace 第7小組專題.Controllers.Checkin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckinController : ControllerBase
    {
        private readonly CheckinService _service;

        public CheckinController(IConfiguration config)
        {
            _service = new CheckinService(config);
        }

        [HttpPost("history")]
        public IActionResult GetByMonth([FromBody]  CheckinQueryRequest request)
        {
            var employeeId = HttpContext.Session.GetInt32("id");
            if (employeeId == null)
                return Unauthorized("尚未登入");

            var data = _service.GetCheckinsByMonth(employeeId.Value, request.month);
            return Ok(data);
        }

        [HttpPost("checkin")]
        public IActionResult CheckIn([FromBody] CheckinRequest request)
        {
            var employeeId = HttpContext.Session.GetInt32("id");
            if (employeeId == null)
                return Unauthorized("尚未登入");

            var result = _service.CheckIn(employeeId.Value);
            return Ok(result);
        }

        [HttpPost("checkout")]
        public IActionResult CheckOut([FromBody] CheckinRequest request)
        {
            var employeeId = HttpContext.Session.GetInt32("id");
            if (employeeId == null)
                return Unauthorized("尚未登入");

            var result = _service.CheckOut(employeeId.Value);
            return Ok(result);
        }

        [HttpPost("today")]
        public IActionResult GetTodayRecord([FromBody] CheckinRequest request)
        {
            var employeeId = HttpContext.Session.GetInt32("id");
            if (employeeId == null)
                return Unauthorized("尚未登入");

            var record = _service.GetRecordByDate(employeeId.Value, DateTime.Today);
            return Ok(record);
        }

        [HttpPost("export-csv")]
        public IActionResult ExportCsv([FromBody] ExportRequest req)
        {
            var employeeId = HttpContext.Session.GetInt32("id");
            if (employeeId == null)
                return Unauthorized("尚未登入");

            var csvBytes = _service.GenerateAttendanceCsv(employeeId.Value, req.Month);
            return File(csvBytes, "text/csv", $"出勤紀錄_{req.Month}.csv");
        }


    }

}
