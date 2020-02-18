﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Verint
using verint_service.Services.Property;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class PropertyController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IPropertyService _propertyService;

        public PropertyController(ILogger<CaseController> logger, IPropertyService propertyService) : base()
        {
            _logger = logger;
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
