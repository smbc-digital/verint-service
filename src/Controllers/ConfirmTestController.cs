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

        /// <summary>
        /// This should delete the VOF - but it seemingly doesn't
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery]string caseId = "699710L5")
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

            var response = await _client.DeleteAsync(new DeleteRequest
            {
                @ref = caseId
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateRequest request = null)
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

            var baseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateRequest>(jsonCreateString);

            var response = await _client.CreateAsync(request ?? baseRequest);

            return Ok(response);
        }


        private const string jsonCreateString = @"
{
  ""caseid"": ""101004228027"",
  ""name"": ""confirm_integrationform"",
  ""data"": {
    ""formdata"": [
      {
          ""name"": ""CONF_POC_CODE"",
          ""item"": ""SHOT""
        },
        {
          ""name"": ""CONF_POC_NAME"",
          ""item"": ""Customer Service Centre""
        },
        {
          ""name"": ""CONF_METH_CODE"",
          ""item"": ""TELE""
        },
        {
          ""name"": ""CONF_CUST_REF"",
          ""item"": ""101003288162""
        },
        {
          ""name"": ""CONF_CUST_TITLE"",
          ""item"": ""Mrs""
        },
        {
          ""name"": ""CONF_CUST_SURNAME"",
          ""item"": ""cranwell""
        },
        {
          ""name"": ""CONF_CUST_PHONE"",
          ""item"": ""07783073470""
        },
        {
          ""name"": ""CONF_CUST_ALT_TEL"",
          ""item"": ""07711591291""
        },
        {
          ""name"": ""CONF_CUST_FAX"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CUST_EMAIL"",
          ""item"": ""karen.cranwell@stockport.gov.uk""
        },
        {
          ""name"": ""CONF_CUST_BUILDING"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CUST_STREETNUM"",
          ""item"": ""26""
        },
        {
          ""name"": ""CONF_CUST_STREET"",
          ""item"": ""HIGHFIELD AVENUE""
        },
        {
          ""name"": ""CONF_CUST_LOCALITY"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CUST_TOWN"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CUST_COUNTY"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CUST_POSTCODE"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CUST_FORENAME"",
          ""item"": ""karen""
        },
        {
          ""name"": ""CONF_ADDRESS_REF"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_SERVICE_CODE"",
          ""item"": ""GREN""
        },
        {
          ""name"": ""CONF_SUBJECT_CODE"",
          ""item"": ""TPOC""
        },
        {
          ""name"": ""le_eventcode"",
          ""item"": ""2002569""
        },
        {
          ""name"": ""le_queue_complete"",
          ""item"": ""AppsConfirmQueuePending""
        },
        {
          ""name"": ""le_associated_obj_type"",
          ""item"": ""D4""
        },
        {
          ""name"": ""le_associated_obj_id"",
          ""item"": ""1002109494""
        },
        {
          ""name"": ""le_description"",
          ""item"": ""tpo""
        },
        {
          ""name"": ""CONF_LOCATION"",
          ""item"": ""This is a test ""
        },
        {
          ""name"": ""CONF_DESC"",
          ""item"": ""tpo""
        },
        {
          ""name"": ""cboclasscode"",
          ""item"": ""SERV""
        },
        {
          ""name"": ""cbocustomertypecode"",
          ""item"": ""PUBL""
        },
        {
          ""name"": ""CONF_ENQ_ID"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CLASSIFICATION"",
          ""item"": ""public_realm-greenspace-tree_preservation_order_check""
        },
        {
          ""name"": ""CONF_ENQ_REF"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_ALT_TEL"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_SITE_CODE"",
          ""item"": ""38101939""
        },
        {
          ""name"": ""CONF_SITE_LOCALITY"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_ASSET_ID"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_ASS_OFF_CODE"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_ASS_OFF_NAME"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_SITE_NAME"",
          ""item"": ""HIGH STREET""
        },
        {
          ""name"": ""CONF_SITE_TOWN"",
          ""item"": ""STOCKPORT""
        },
        {
          ""name"": ""CONF_SITE_COUNTY"",
          ""item"": ""STOCKPORT""
        },
        {
          ""name"": ""CONF_X_COORD"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_Y_COORD"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_JOB_NUM"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_ASS_OFF_PHONE"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_JOB_START"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_JOB_END"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CONTACT"",
          ""item"": "" cranwell""
        },
        {
          ""name"": ""CONF_CONTACT_EMAIL"",
          ""item"": ""karen.cranwell@stockport.gov.uk""
        },
        {
          ""name"": ""CONF_STATUS_CODE"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_STATUS_NAME"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_CONTACT_PHONE"",
          ""item"": ""07783073470""
        },
        {
          ""name"": ""CONF_CONTACT_FAX"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_LOGGED_BY"",
          ""item"": ""KAREN""
        },
        {
          ""name"": ""CONF_USERNAME"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_LOGGED_TIME"",
          ""item"": ""2020-06-09T15:37:46.380Z""
        },
        {
          ""name"": ""CONF_EFFECTIVE_TIME"",
          ""item"": """"
        },
        {
          ""name"": ""CONF_STATUS_NOTES"",
          ""item"": ""Logged by: KAREN""
        },
        {
          ""name"": ""CONF_FOLLOW_UP_BY"",
          ""item"": """"
        }
    ]
  }
}";
    }
}