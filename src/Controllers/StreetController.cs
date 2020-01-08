using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Addresses;
using verint_service.Services.Street;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class StreetController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IStreetService _streetService;

        public StreetController(ILogger<CaseController> logger, IStreetService streetService) : base()
        {
            _logger = logger;
            _streetService = streetService;
        }

        [HttpGet]
        [Route("streetsearch/{street}")]
        public async Task<IEnumerable<AddressSearchResult>> StreetSearch(string street)
        {
            return await _streetService.SearchByStreetAsync(street);
        }
    }
}
