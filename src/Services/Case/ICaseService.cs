using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Models;

namespace verint_service.Services.Case
{
    public interface ICaseService
    {
        Task<StockportGovUK.NetStandard.Models.Verint.Case> GetCase(string caseId);

        Task<string> CreateCase(StockportGovUK.NetStandard.Models.Verint.Case crmCase);

        Task<int> UpdateCaseDescription(StockportGovUK.NetStandard.Models.Verint.Case crmCase);

        Task CreateNotesWithAttachment(NoteWithAttachments note);
    }
}