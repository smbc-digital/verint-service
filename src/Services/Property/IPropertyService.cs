using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Verint;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace verint_service.Services.Property
{
    public interface IPropertyService
    {
        Task<IEnumerable<AddressSearchResult>> SearchByPostcodeAsync(string postcode);
    }
}
