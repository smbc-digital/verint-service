using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Addresses;
using verint_service.Models;
using verint_service.Services.Street;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class StreetController : ControllerBase
    {
        private readonly IStreetService _streetService;

        public StreetController(IStreetService streetService)
        {
            _streetService = streetService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<AddressSearchResult> Get(string id)
        {
            return await _streetService.GetStreet(id);
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult> Search([FromQuery]StreetSearch search)
        {
            var result = await _streetService.Search(search);

            if (result == null || !result.Any())
            {
                return NotFound();
            }
            
            return Ok(result);
        }

        [HttpGet]
        [Route("streetsearch/{street}")]
        public async Task<IEnumerable<AddressSearchResult>> StreetSearch(string street)
        {
            return await _streetService.SearchByStreetAsync(street);
        }

        [HttpGet]
        [Route("usrnsearch/{usrn}")]
        public async Task<IEnumerable<AddressSearchResult>> UsrnSearch(string usrn)
        {
            return await _streetService.SearchByUsrnAsync(usrn);
        }
    }
}
