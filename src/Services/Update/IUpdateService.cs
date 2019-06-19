using System.Threading.Tasks;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services.Update
{
    public interface IUpdateService
    {
       Task<writeCaseEformDataResponse> UpdateIntegrationFormFields(IntegrationFormFieldsUpdateEntity updateEntity);
    }
}
