using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services
{
    public interface IIndividualService
    {
        Task<FWTObjectID> ResolveAsync(Customer customer);
        
        Task UpdateIndividual(FWTIndividual individual, Customer customer);
    }
}