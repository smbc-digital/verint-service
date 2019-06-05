using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("[Controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var name = Assembly.GetEntryAssembly()?.GetName().Name;
            var version = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
            return Ok($"{{'AppVersion': '{version}', 'Name': '{name}'}}");
        }
    }
}