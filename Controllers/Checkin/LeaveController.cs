using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using 第7小組專題.Models.Checkin;
using 第7小組專題.Services.Checkin;

namespace 第7小組專題.Controllers.Checkin
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly LeaveService _service;

        public LeaveController(IConfiguration config)
        {
            _service = new LeaveService(config);
        }

        [HttpPost("apply")]
        public IActionResult Apply([FromForm] LeaveApplyRequest request, IFormFile? attachment)
        {
            var employeeId = HttpContext.Session.GetInt32("id");
            if (employeeId == null)
                return Unauthorized("尚未登入");
            request.employeeId = employeeId.Value;

            var result = _service.ApplyLeave(request, attachment);
            return Ok(result);
        }

        [HttpPost("my-records")]
        public IActionResult GetMyLeaveRecords([FromBody] LeaveQueryRequest req)
        {
            var employeeId = HttpContext.Session.GetInt32("id");
            if (employeeId == null)
                return Unauthorized("尚未登入");
            req.employeeId = employeeId.Value;

            var data = _service.GetLeaveRecordsByEmployee(req);
            return Ok(data);
        }



    }
}
