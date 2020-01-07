using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Models.Verint;
using verint_service.Services.Organisation;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class OrganisationController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IOrganisationService _organisationService;

        public OrganisationController(ILogger<CaseController> logger, IOrganisationService organisationService) : base()
        {
            _logger = logger;
            _organisationService = organisationService;
        }

        [HttpGet]
        public async Task<IEnumerable<Models.Organisation>> SearchByOrganisationAsync(string organisationName)
        {
            return await _organisationService.SearchByOrganisationAsync(organisationName);
        }
    }
}
