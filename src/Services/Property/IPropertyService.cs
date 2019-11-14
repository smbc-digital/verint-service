using StockportGovUK.NetStandard.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace verint_service.Services.Property
{
    public interface IPropertyService
    {
        Task<IEnumerable<AddressSearchResult>> SearchByPostcodeAsync(string postcode);
    }
}
