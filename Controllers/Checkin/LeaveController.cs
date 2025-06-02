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
            var result = _service.ApplyLeave(request, attachment);
            return Ok(result);
        }

        [HttpPost("my-records")]
        public IActionResult GetMyLeaveRecords([FromBody] LeaveQueryRequest req)
        {
            var data = _service.GetLeaveRecordsByEmployee(req);
            return Ok(data);
        }



    }
}
