using System.Threading.Tasks;

namespace verint_service.Services.Case
{
    public interface ICaseService
    {
        Task<Models.Case> GetCase(string caseId);
    }
}