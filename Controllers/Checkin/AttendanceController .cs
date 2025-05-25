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
            var repo = new AttendanceRepository(config);
            _service = new AttendanceService(repo);
        }

        [HttpPost("yearly")]
        public IActionResult GetYearlyAttendance([FromBody] AttendanceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.employeeId))
                return BadRequest("缺少員工代碼");

            var data = _service.GetMonthlyAttendanceDays(request.year, request.employeeId);
            return Ok(data);
        }
    }

 }
