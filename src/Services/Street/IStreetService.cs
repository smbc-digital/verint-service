using StockportGovUK.NetStandard.Models.Addresses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace verint_service.Services.Street
{
    public interface IStreetService
    {
        Task<IEnumerable<AddressSearchResult>> SearchByStreetAsync(string reference);
        Task<IEnumerable<AddressSearchResult>> SearchByUsrnAsync(string usrn);
        Task<AddressSearchResult> GetStreet(string reference);
    }
}
