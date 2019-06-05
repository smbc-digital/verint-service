using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
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
            var name = Assembly.GetEntryAssembly().GetName().Name;
            var assembly = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "verint-service.dll");
            var version = FileVersionInfo.GetVersionInfo(assembly).FileVersion;
            return Ok($"{{'Verson': '{version}', 'Name': '{name}'}}");
        }
    }
}