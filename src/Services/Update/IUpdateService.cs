using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Models.Verint.Update;
using VerintWebService;

namespace verint_service.Services.Update
{
    public interface IUpdateService
    {
       Task<writeCaseEformDataResponse> UpdateIntegrationFormFields(IntegrationFormFieldsUpdateModel updateEntity);
    }
}