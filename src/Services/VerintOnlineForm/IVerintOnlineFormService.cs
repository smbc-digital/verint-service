using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
using System.Threading.Tasks;

namespace verint_service.Services.VerintOnlineForm
{
    public interface IVerintOnlineFormService
    {
        Task<VerintOnlineFormResponse> CreateVOFCase(VerintOnlineFormRequest model);
    }
}
