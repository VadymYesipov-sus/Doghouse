using Microsoft.AspNetCore.Mvc;

namespace Doghouse.Controllers
{
    //inheritance of ControllerBase is better than COntroller if we don't need to serve views
    [ApiController]
    public class PingController : ControllerBase 
    {
        [HttpGet]
        [Route("ping")]
        public ActionResult<string> Ping()
        {
            return Ok("Dogshouseservice.Version1.0.1");
        }
    }
}
