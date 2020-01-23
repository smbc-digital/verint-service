using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;

namespace verint_service.Services.Case
{
    public interface ICaseService
    {
        Task<StockportGovUK.NetStandard.Models.Verint.Case> GetCase(string caseId);

        Task<string> CreateCase(Case crmCase);

        Task<int> UpdateCaseDescription(Case crmCase);

        Task CreateNotesWithAttachment(NoteWithAttachments note);

        Task<string> CreateCase(StockportGovUK.NetStandard.Models.Verint.Case crmCase);

    }
}