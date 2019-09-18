using System.Threading.Tasks;
using VerintWebService;

namespace verint_service.Services.Case
{
    public interface ICaseService
    {
        Task<Models.Case> GetCase(string caseId);

        Task<string> CreateCase(Models.Case crmCase);
    }
}