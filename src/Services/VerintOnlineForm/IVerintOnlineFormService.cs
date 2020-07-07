using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
using System.Threading.Tasks;
using VOFWebService;

namespace verint_service.Services.VerintOnlineForm
{
    public interface IVerintOnlineFormService
    {
        Task<VerintOnlineFormResponse> CreateVOFCase(VerintOnlineFormRequest model);

        Task<GetResponse1> GetVOFCase(string verintOnlineFormReference);
    }
}
