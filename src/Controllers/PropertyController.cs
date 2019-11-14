﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Addresses;
using verint_service.Services.Property;

namespace verint_service.Controllers
{
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
        [Route("/search/{postcode}")]
        public async Task<IEnumerable<AddressSearchResult>> Search(string postcode)
        {
            return await _propertyService.SearchByPostcodeAsync(postcode);
        }
    }
}
