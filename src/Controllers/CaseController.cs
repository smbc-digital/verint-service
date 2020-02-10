using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Models.Verint.Update;
using verint_service.ModelBinders;
using verint_service.Models;
using verint_service.Services.Case;
using verint_service.Services.Event;
using verint_service.Services.Update;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication(IgnoredRoutes = new []{"/api/v1/case/event"})]
    public class CaseController : ControllerBase
    {
        private readonly ICaseService _caseService;
        private readonly IUpdateService _updateService;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IEventService _eventService;

        public CaseController(ICaseService caseService, IUpdateService updateService, ILogger<CaseController> logger, IEventService eventService)
        {
            _caseService = caseService;
            _updateService = updateService;
            _logger = logger;
            _eventService = eventService;

            var proxyHttpClientHandler = new HttpClientHandler {
	            Proxy = new WebProxy(new Uri("http://172.16.0.126:8080"), BypassOnLocal: false),
	            UseProxy = true
            };

            _httpClient = new HttpClient(proxyHttpClientHandler);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string caseId)
        {
            var verintCase = await _caseService.GetCase(caseId);

            return Ok(verintCase);
        }

        [HttpPut]
        public IActionResult Update()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Case crmCase)
        {
            try
            {
                var response = await _caseService.CreateCase(crmCase);

                return CreatedAtAction("Create", response);
            }
            catch (Exception ex)
            {
                _logger.LogError("CaseController.Create: Failed to create crm case", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Method to append a payment status to the description
        /// </summary>
        /// <param name="crmCase">The case to be updated</param>
        /// <param name="toBeAppended"> bool to indicate if the status should be added too or replaced</param>
        /// <returns>An int declaring the state of the update</returns>
        [HttpPost]
        [Route("updatecasedescription")]
        public async Task<IActionResult> UpdateCaseDescription(Case crmCase, bool toBeAppended)
        {
            try
            {
                var response = await _caseService.UpdateCaseDescription(crmCase, toBeAppended);

                return CreatedAtAction("UpdateCaseDescription", response);
            }
            catch (Exception ex)
            {
                _logger.LogError("CaseController.UpdateCaseDescription: Failed to update crm case description", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
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
            catch (Exception ex)
            {
                _logger.LogError("CaseController.UpdateIntegrationFormFields: Failed to update crm fields case", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("event")]
        public void CaseEventHandler([ModelBinder(typeof(CaseEventModelBinder))]CaseEventModel model)
        {
            _eventService.HandleCaseEvent(model);
        }
    }
}