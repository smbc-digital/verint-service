using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Services
{
    public interface IIndividualService
    {
        Task<FWTObjectID> ResolveIndividual(Customer customer);
        Task UpdateIndividual(FWTIndividual individual, Customer customer);

        Task<string> CheckUPRNForId(Customer customer);
    }
}