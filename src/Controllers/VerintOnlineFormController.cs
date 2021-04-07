using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
using System;
using System.Threading.Tasks;
using verint_service.Attributes;
using verint_service.Services.VerintOnlineForm;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class VerintOnlineFormController : ControllerBase
    {
        private readonly IVerintOnlineFormService _verintOnlineFormService;
        private readonly ILogger<VerintOnlineFormController> _logger;

        public VerintOnlineFormController(IVerintOnlineFormService verintOnlineFormService, ILogger<VerintOnlineFormController> logger)
        {
            _verintOnlineFormService = verintOnlineFormService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a Verint Online From, Verint Case, and triggers Confirm integration
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(VerintOnlineFormRequest model)
        {
            try
            {
                var result = await _verintOnlineFormService.CreateVOFCase(model);
                return Ok(result);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
            

        /// <summary>
        /// Gets a verint online form case
        /// </summary>
        [HttpGet]
        [DevelopmentOnly]
        public async Task<IActionResult> GetCase(string verintOnlineFormReference)
            => Ok(await _verintOnlineFormService.GetVOFCase(verintOnlineFormReference));

        [HttpPatch]
        [DevelopmentOnly]
        public IActionResult TestLogger(string valueToLog = "test")
        {
            _logger.LogDebug($"VerintOnlineFormController.TestLogger: {valueToLog}");
            _logger.LogInformation($"VerintOnlineFormController.TestLogger: {valueToLog}");
            _logger.LogWarning($"VerintOnlineFormController.TestLogger: {valueToLog}");
            _logger.LogError($"VerintOnlineFormController.TestLogger: {valueToLog}");
            _logger.LogCritical($"VerintOnlineFormController.TestLogger: {valueToLog}");

            return Ok();
        }
    }
}
