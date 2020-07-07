using VerintWebService;
using VOFWebService;

namespace verint_service.Helpers.VerintConnection
{
    public interface IVerintConnection
    {
        IVerintClient Client();

        IVOFClient VOFClient();
    }
}