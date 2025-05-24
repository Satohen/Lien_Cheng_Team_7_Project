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
            var data = _service.GetCheckinsByMonth(request.employeeId, request.month);
            return Ok(data);
        }

        [HttpPost("checkin")]
        public IActionResult CheckIn([FromBody] CheckinRequest request)
        {
            var result = _service.CheckIn(request.employeeId);
            return Ok(result);
        }

        [HttpPost("checkout")]
        public IActionResult CheckOut([FromBody] CheckinRequest request)
        {
            var result = _service.CheckOut(request.employeeId);
            return Ok(result);
        }

        [HttpPost("today")]
        public IActionResult GetTodayRecord([FromBody] CheckinRequest request)
        {
            var record = _service.GetRecordByDate(request.employeeId, DateTime.Today);
            return Ok(record);
        }



    }

}
