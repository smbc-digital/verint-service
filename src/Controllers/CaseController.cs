using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Models.Verint.Update;
using verint_service.ModelBinders;
using verint_service.Models;
using verint_service.Services.Case;
using verint_service.Services.Create;
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
        private readonly ICreateService _createService;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IEventService _eventService;

        public CaseController(ICaseService caseService, IUpdateService updateService, ILogger<CaseController> logger, IEventService eventService, ICreateService createService)
        {
            _caseService = caseService;
            _updateService = updateService;
            _logger = logger;
            _eventService = eventService;
            _createService = createService;

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
        [Route("create-case")]
        public async Task<IActionResult> Create(Case crmCase)
        {
            try
            {
                var response = await _createService.CreateCase(crmCase);
                return Ok(response);
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("testhttps")]
        public async Task<IActionResult> TestHttps()
        {
            try{
                var result = await _httpClient.GetAsync("https://google.com");
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpGet]
        [Route("testhttp")]
        public async Task<IActionResult> TestHttp()
        {
            try{
                var result = await _httpClient.GetAsync("http://google.com");
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(ex);
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