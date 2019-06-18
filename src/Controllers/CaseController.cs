using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using verint_service.Services.Case;
using verint_service.Models;
using verint_service.Services.Update;
using VerintWebService;

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
            updateEntity = new IntegrationFormFieldsUpdateEntity
            {
                CaseReference = "",
                IntegrationFormName = "",
                IntegrationFormFields =  new List<IntegrationFormField>
                {
                    new IntegrationFormField
                    {
                        FormFieldName = "firstname",
                        FormFieldValue = "TESTUPDATE FIRSTNAME"
                    }
                }
            };

            var verintCase = await _caseService.GetCase(updateEntity.CaseReference);
            verintCase.SetCustomAttribute("firstname", "TESTUPDATE");

            var eformData = new FWTCaseEformData();

            var caseEformInstance = new FWTCaseEformInstance
            {
                CaseReference = updateEntity.CaseReference,
                EformName = updateEntity.IntegrationFormName
            };

            var formFields = new FWTEformField[0];

            if (verintCase.IntegrationFormFields != null && verintCase.IntegrationFormFields.Any())
            {
                formFields = new FWTEformField[verintCase.IntegrationFormFields.Count];
                var count = 0;

                foreach (var field in verintCase.IntegrationFormFields)
                {
                    var caseFormField = new FWTEformField
                    {
                        FieldValue = field.Value ?? string.Empty,
                        FieldName = field.Name
                    };
                    // If value is null, set to empty string
                    formFields[count] = caseFormField;
                    count = count + 1;
                }
            }

            eformData.CaseEformInstance = caseEformInstance;
            eformData.EformData = formFields;

            var result = await _updateService.UpdateIntegrationFormField(eformData);

            return Ok();
        }
    }
}