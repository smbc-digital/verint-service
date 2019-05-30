using System;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class HealthcheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var name = Assembly.GetEntryAssembly().GetName().Name;
            var version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            return Ok($"{{'Verson': '{version}', 'Name': '{name}'}}");
        }
    }
}