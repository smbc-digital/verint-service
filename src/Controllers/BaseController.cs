using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Models.Verint.Update;
using verint_service.ModelBinders;
using verint_service.Models;
using verint_service.Services.Case;
using verint_service.Services.Event;
using verint_service.Services.Update;

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
