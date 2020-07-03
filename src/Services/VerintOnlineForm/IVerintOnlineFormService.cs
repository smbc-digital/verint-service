using System.Threading.Tasks;
using static verint_service.Controllers.VerintOnlineFormController;

namespace verint_service.Services.VerintOnlineForm
{
    public interface IVerintOnlineFormService
    {
        Task<VerintOnlineFormResponse> CreateVOFCase(VerintOnlineFormRequest model);
    }
}
