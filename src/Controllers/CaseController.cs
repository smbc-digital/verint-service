using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using verint_service.Services.Case;
using verint_service.Models;
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

        public CaseController(ICaseService caseService, IUpdateService updateService)
        {
            _caseService = caseService;
            _updateService = updateService;
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
        public async Task<IActionResult> UpdateIntegrationFormFields(IntegrationFormFieldsUpdateEntity updateEntity)
        {
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
    }
}