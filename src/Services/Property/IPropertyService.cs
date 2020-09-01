using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Verint;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace verint_service.Services.Property
{
    public interface IPropertyService
    {
        Task<IEnumerable<AddressSearchResult>> SearchByPostcodeAsync(string postcode);

        Task<StockportGovUK.NetStandard.Models.Verint.Address> GetPropertyAsync(string id);

        Task<string> CheckUPRNForId(StockportGovUK.NetStandard.Models.Verint.Address address);
    }
}
