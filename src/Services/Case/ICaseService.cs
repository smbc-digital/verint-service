using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;

namespace verint_service.Services.Case
{
    public interface ICaseService
    {
        Task<StockportGovUK.NetStandard.Models.Verint.Case> GetCase(string caseId);

        Task<string> Create(StockportGovUK.NetStandard.Models.Verint.Case crmCase);

        Task<int> UpdateDescription(StockportGovUK.NetStandard.Models.Verint.Case crmCase);

        Task CreateNotesWithAttachment(NoteWithAttachments note);

        Task CreateNote(NoteRequest noteRequest);
        
        Task<bool> AddCaseFormField(string caseId, string key, string value);

        Task<string> Close(string id, string reasonTitle, string description);

        Task WriteCachedNotes(string id);
        
        Task<int> UpdateTitle(StockportGovUK.NetStandard.Models.Verint.Case crmCase);
    }
}