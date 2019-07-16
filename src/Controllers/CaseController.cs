using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Models.Verint.Update;
using verint_service.Services.Case;
using verint_service.Services.Update;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class CaseController : ControllerBase
    {
        private readonly ICaseService _caseService;
        private readonly IUpdateService _updateService;
        private readonly ILogger<CaseController> _logger;
        private readonly HttpClient _httpClient;

        public CaseController(ICaseService caseService, IUpdateService updateService, ILogger<CaseController> logger)
        {
            _caseService = caseService;
            _updateService = updateService;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string caseId)
        {
            _logger.LogWarning($"**DEBUG: CaseController:GetCase, Makeing rrquest to getCase with caseId {caseId}");
            var verintCase = await _caseService.GetCase(caseId);

            return Ok(verintCase);
        }

        [HttpPut]
        public IActionResult Update()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult Create()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery]string caseId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch]
        [Route("integration-form-fields")]
        public async Task<IActionResult> UpdateIntegrationFormFields([FromBody]IntegrationFormFieldsUpdateModel updateEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _updateService.UpdateIntegrationFormFields(updateEntity);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> TestHttps()
        {
            var result = await _httpClient.GetAsync("https://google.com");
            return Ok(result);
        }
    }
}