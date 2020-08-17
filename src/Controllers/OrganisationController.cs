using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Verint;
using StockportGovUK.NetStandard.Models.Verint.Lookup;
using verint_service.Services.Organisation;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class OrganisationController : ControllerBase
    {
        private readonly ILogger _logger;
        
        private readonly IOrganisationService _organisationService;

        public OrganisationController(ILogger<OrganisationController> logger, IOrganisationService organisationService)
        {
            _logger = logger;
            _organisationService = organisationService;
        }

        /// <summary>
        /// Gets an existing orgnisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Organisation>> Get(string id)
        {
            var organisation = await _organisationService.GetAsync(id);
            if(organisation!=null)
            {
                return Ok(organisation);    
            }
            
            return NotFound();
        }

        /// <summary>
        /// Create a new organisation 
        /// </summary>
        /// <param name="organisation"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(Organisation organisation)
        {
            var results = await _organisationService.CreateAsync(organisation);
            organisation = await _organisationService.GetAsync(results.ObjectReference[0]);
            return CreatedAtAction("Create", organisation);
        }
        
        /// <summary>
        /// Matches the specified organisation to the closest accurate matching organisation in verint
        /// </summary>
        /// <param name="organisation"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/Match")]
        public async Task<ActionResult<Organisation>> Match(Organisation organisation)
        {
            var results = await _organisationService.MatchAsync(organisation);
            _logger.LogDebug($"OrganisationController.Match Found - Organisation: { organisation.Name }");

            if(results== null)
            {
                _logger.LogDebug($"OrganisationController.Match Not Found - Organisation: { organisation.Name }");
                return NotFound();    
            }
            
            var matchingOrg = await _organisationService.GetAsync(results.ObjectReference[0]);
            _logger.LogDebug($"OrganisationController.Match GetAsync { results.ObjectReference[0] }");
            return Ok(matchingOrg);    
        }

        /// <summary>
        /// Matches the specified organisation to the closest accurate matching organisation in verint
        /// If no match is found a new organisation is created
        /// </summary>
        /// <param name="organisation"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/Resolve")]
        public async Task<ActionResult<Organisation>> Resolve(Organisation organisation)
        {
            var results = await _organisationService.ResolveAsync(organisation);
            _logger.LogDebug($"OrganisationController.Resolve Found - Organisation: { organisation.Name }");

            if(results!= null)
            {
                var matchingOrg = await _organisationService.GetAsync(results.ObjectReference[0]);
                _logger.LogDebug($"OrganisationController.Resolve GetAsync { results.ObjectReference[0] }");
                return Ok(matchingOrg);    
            }
            
            _logger.LogDebug($"OrganisationController.Resolve Not Found - Organisation: { organisation.Name }");
            return NotFound();
        }

        /// <summary>
        /// Searches existing orgnisations by name
        /// </summary>
        /// <param name="organisation"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("search/{organisation}")]
        public async Task<ActionResult<IEnumerable<OrganisationSearchResult>>> Search(string organisation)
        {
            try
            {
                var results = await _organisationService.SearchByNameAsync(organisation);
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