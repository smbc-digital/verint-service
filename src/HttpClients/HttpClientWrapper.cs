﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace verint_service.HttpClients
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _client = new HttpClient();

        public void SetHttpClientSecurityHeader(string authToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authToken);
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, object content)
        {
            var encodedContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            return await _client.PostAsync(endpoint, encodedContent);
        }
    }
}
