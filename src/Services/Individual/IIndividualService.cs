using System.Threading.Tasks;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services
{
    public interface IIndividualService
    {
        Task<FWTObjectID> ResolveIndividual(Customer customer);
        Task UpdateIndividual(FWTIndividual individual, Customer customer);
    }
}