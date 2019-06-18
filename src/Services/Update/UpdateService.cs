using System.Linq;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using VerintWebService;

namespace verint_service.Services.Update
{
    public class UpdateService : IUpdateService
    {
        private readonly IVerintClient _verintConnection;

        public UpdateService(IVerintConnection verint)
        {
            _verintConnection = verint.Client();
        }

        public async Task<writeCaseEformDataResponse> UpdateIntegrationFormField(FWTCaseEformData eformData)
        {
            return await _verintConnection.writeCaseEformDataAsync(eformData);
        }
    }
}
