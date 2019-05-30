using System.Threading.Tasks;
using verint_service.Models;

namespace verint_service.Services
{
    public interface ICaseService
    {
        Task<Case> GetCase(string caseId);
    }
}