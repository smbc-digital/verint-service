using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.AspNetCore.Availability.Attributes;
using StockportGovUK.AspNetCore.Availability.Managers;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class ValuesController : ControllerBase
    {
        private AvailabilityManager _availabilityManager;

        public ValuesController(AvailabilityManager availabilityManager)
        {
            _availabilityManager = availabilityManager;
        }

        [HttpGet]
        [FeatureToggle(FeatureToggles.MyToggle)]
        public IActionResult Get()
        {
            return Ok("{'value1': 1, 'value2': 2}");
        }

        [HttpPost]
        [OperationalToggle(OperationalToggles.MyToggle)]
        public async Task<IActionResult> Post()
        {
            if(await _availabilityManager.IsFeatureEnabled(FeatureToggles.MyToggle))
            {
                return Ok("{'value1': 1, 'value2': 2}");
            }
            
            return NotFound();            
        }
    }
}