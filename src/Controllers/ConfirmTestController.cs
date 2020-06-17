using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using VOFWebService;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication(IgnoredRoutes = new[] { "/api/v1/case/event" })]
    public class ConfirmTestController : ControllerBase
    {
        private readonly IEndpointBehavior _requestBehavior;

        public ConfirmTestController(
            IEndpointBehavior requestBehavior)
        {
            _requestBehavior = requestBehavior;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string caseId = "699710L5")
        {
            var endpointAddress = new EndpointAddress("http://scnverinttest.stockport.gov.uk:9081/service/service.wsdl"); //http://scnverinttest:9081/service/service.wsdl
            var _httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                Name = "VOFWebBinding",
                MaxReceivedMessageSize = 67108864,
                CloseTimeout = new TimeSpan(0, 10, 0),
                OpenTimeout = new TimeSpan(0, 10, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                MaxBufferPoolSize = 67108864,
                MaxBufferSize = 67108864,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Buffered
            };
            var _client = new serviceClient(_httpBinding, endpointAddress);

            _client.Endpoint.EndpointBehaviors.Add(_requestBehavior);

            var response = await _client.GetAsync(new GetRequest
            {
                @ref = caseId
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateRequest request)
        {
            var endpointAddress = new EndpointAddress("http://scnverinttest.stockport.gov.uk:9081/service/service.wsdl"); //http://scnverinttest:9081/service/service.wsdl
            var _httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                Name = "VOFWebBinding",
                MaxReceivedMessageSize = 67108864,
                CloseTimeout = new TimeSpan(0, 10, 0),
                OpenTimeout = new TimeSpan(0, 10, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                MaxBufferPoolSize = 67108864,
                MaxBufferSize = 67108864,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Buffered
            };
            var _client = new serviceClient(_httpBinding, endpointAddress);

            _client.Endpoint.EndpointBehaviors.Add(_requestBehavior);

            var response = await _client.CreateAsync(request);

            return Ok(response);
        }
    }
}