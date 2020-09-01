using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;

namespace verint_service.Services
{
    public interface IInteractionService
    {    
        Task<long> CreateAsync(StockportGovUK.NetStandard.Models.Verint.Case crmCase);
    }
}