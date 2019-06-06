using VerintWebService;

namespace verint_service.Helpers.VerintConnection
{
    public interface IVerintConnection
    {
        IVerintClient Client();
    }
}