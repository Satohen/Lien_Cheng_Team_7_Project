using Microsoft.AspNetCore.Mvc;
using 第7小組專題.Models.Checkin;
using 第7小組專題.Repository.Checkin;
using 第7小組專題.Services.Checkin;

namespace 第7小組專題.Controllers.Checkin
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _service;

        public AttendanceController(IConfiguration config)
        {
            _service = new AttendanceService(config);

        }

        [HttpPost("yearly")]
        public IActionResult GetYearlyAttendance([FromBody] AttendanceRequest request)
        {

            var employeeId = HttpContext.Session.GetInt32("id");
            if (employeeId == null)
                return Unauthorized("尚未登入");

            if (string.IsNullOrWhiteSpace(request.employeeId))
                return BadRequest("缺少員工代碼");

            var result = _service.GetMonthlySummary(request.year, employeeId.Value);
            return Ok(result);
        }

    }

}
