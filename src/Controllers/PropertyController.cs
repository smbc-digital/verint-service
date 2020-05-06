using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Addresses;
using verint_service.Services.Property;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        [Route("search/{postcode}")]
        public async Task<IEnumerable<AddressSearchResult>> Search(string postcode)
        {
            return await _propertyService.SearchByPostcodeAsync(postcode);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<StockportGovUK.NetStandard.Models.Verint.Address> Get(string id)
        {
            return await _propertyService.GetPropertyAsync(id);
        }
    }
}
