using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var data = _service.GetAllCheckins();
            return Ok(data);
        }
    }
}
