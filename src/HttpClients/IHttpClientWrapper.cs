using System.Net.Http;
using System.Threading.Tasks;

namespace verint_service.HttpClients
{
    public interface IHttpClientWrapper
    {
        void SetHttpClientSecurityHeader(string authToken);
        Task<HttpResponseMessage> PostAsync(string endpoint, object content);
    }
}
