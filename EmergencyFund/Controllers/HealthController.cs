using Microsoft.AspNetCore.Mvc;

namespace EmergencyFund.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        public record HealthResponse(string Status);


        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new HealthResponse("ok"));
        }
    }
}
