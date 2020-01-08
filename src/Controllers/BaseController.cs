using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace verint_service.Controllers
{
    public class BaseController : Controller
    {
        public readonly HttpClient _httpClient;

        public BaseController()
        {
            var proxyHttpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy(new Uri("http://172.16.0.126:8080"), BypassOnLocal: false),
                UseProxy = true
            };

            _httpClient = new HttpClient(proxyHttpClientHandler);
        }
    }
}
