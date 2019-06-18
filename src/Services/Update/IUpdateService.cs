using System.Threading.Tasks;
using VerintWebService;

namespace verint_service.Services.Update
{
    public interface IUpdateService
    {
       Task<writeCaseEformDataResponse> UpdateIntegrationFormField(FWTCaseEformData verintCase);
    }
}
