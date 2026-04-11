using Microsoft.AspNetCore.Mvc;

namespace HooperMedia.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController(ILogger<StatusController> logger) : ControllerBase
    {
        [HttpGet(Name = "AppStatus")]
        public ActionResult<string> Get()
        {
            logger.LogInformation("Status verified");
            return Ok("Application is running successfully");
        }
    }
}
