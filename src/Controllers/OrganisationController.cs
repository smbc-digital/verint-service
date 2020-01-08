using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using verint_service.Models;
using verint_service.Services.Organisation;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class OrganisationController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IOrganisationService _organisationService;

        public OrganisationController(ILogger<OrganisationController> logger, IOrganisationService organisationService) : base()
        {
            _logger = logger;
            _organisationService = organisationService;
        }

        [HttpGet]
        public async Task<ActionResult<Organisation>> Get([Required][FromQuery]string organisation)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("search/{organisation}")]
        public async Task<ActionResult<IEnumerable<Organisation>>> Search(string organisation)
        {
            try
            {
                var results = await _organisationService.SearchByOrganisationAsync(organisation);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganisationController.Search: Failed to search for organisation", ex.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}