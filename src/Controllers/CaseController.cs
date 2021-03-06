using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Verint;
using StockportGovUK.NetStandard.Models.Verint.Update;
using verint_service.Models.CaseEvent;
using verint_service.Models.CaseEvent.Binders;
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
        private readonly IEventService _eventService;

        public CaseController(ICaseService caseService, IUpdateService updateService, ILogger<CaseController> logger, IEventService eventService)
        {
            _caseService = caseService;
            _updateService = updateService;
            _logger = logger;
            _eventService = eventService;
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
                var stopwatch = Stopwatch.StartNew();
                var response = await _caseService.Create(crmCase);
                return CreatedAtAction("Create", response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CaseController.Create: Failed to create crm case {crmCase.ID} - {ex.Message}, ", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Method to append a payment status to the description
        /// </summary>
        /// <param name="crmCase">The case to be updated</param>
        /// <returns>An int declaring the state of the update</returns>
        [HttpPost]
        [Route("updatecasedescription")]
        public async Task<IActionResult> UpdateCaseDescription(Case crmCase)
        {
            try
            {
                var response = await _caseService.UpdateDescription(crmCase);

                return CreatedAtAction("UpdateCaseDescription", response);
            }
            catch (Exception ex)
            {
                _logger.LogError("CaseController.UpdateCaseDescription: Failed to update crm case description", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Method to update case title
        /// </summary>
        /// <param name="crmCase">The case to be updated</param>
        /// <returns>An int declaring the state of the update</returns>
        [HttpPost]
        [Route("update-case-title")]
        public async Task<IActionResult> UpdateCaseTitle(Case crmCase)
        {
            try
            {
                var response = await _caseService.UpdateTitle(crmCase);

                return CreatedAtAction("UpdateCaseTitle", response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CaseController.UpdateCaseTitle: Failed to update crm case title: {crmCase.CaseReference}", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Adds a case form field the specified case
        /// </summary>
        /// <param name="request"></param>
        /// <returns>An int declaring the state of the update</returns>
        [HttpPatch]
        [Route("add-caseform-field")]
        public async Task<IActionResult> AddCaseFormField([FromBody]AddCaseFormFieldRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                return CreatedAtAction("AddCaseFormField", await _caseService.AddCaseFormField(request.CaseReference, request.Key, request.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError("CaseController.AddCaseFormField: Failed to add case form field", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery]string caseId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch]
        [Route("close-case")]
        public async Task<IActionResult> CloseCase([FromBody]CloseCaseRequest closeCaseRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                await _caseService.Close(closeCaseRequest.CaseReference, closeCaseRequest.ReasonTitle, closeCaseRequest.Description);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("CaseController.CloseCase: Failed to cloase crm case - {closeCaseRequest.CaseReference}", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch]
        [Route("integration-form-fields")]
        public async Task<IActionResult> UpdateIntegrationFormFields([FromBody]IntegrationFormFieldsUpdateModel updateEntity)
        {
            if (!ModelState.IsValid)
                return BadRequest();

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

        [HttpPost]
        [Route("add-note-with-attachments")]
        public async Task<IActionResult> AddNoteWithAttachments([FromBody] NoteWithAttachments model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                await _caseService.CreateNotesWithAttachment(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("CaseController.AddNoteWithAttachments: Failed to create note with attachments", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("add-note")]
        public async Task<IActionResult> AddNote([FromBody] NoteRequest noteRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                await _caseService.CreateNote(noteRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("CaseController.AddNoteWithAttachments: Failed to create note with attachments", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}