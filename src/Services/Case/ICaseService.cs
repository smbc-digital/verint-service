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
    }
}