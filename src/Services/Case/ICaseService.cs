using System.Threading.Tasks;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services.Case
{
    public interface ICaseService
    {
        Task<Models.Case> GetCase(string caseId);

        Task<string> CreateCase(Models.Case crmCase);

        Task<int> UpdateCaseDescription(Models.Case crmCase);

        Task CreateNotesWithAttachment(NoteWithAttachments note);
    }
}