using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Verint;
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

        /// <summary>
        /// Gets VOF data based on confirm caseId
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a case with same basic data for a 'confirm_integrationform' form
        /// 
        /// DateTime fields threw an exception
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery]string caseId)
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
            baseRequest.caseid = caseId;

            var formdata = baseRequest.data.formdata.ToList();
            formdata.Add(new Field { Item = "18/06/2020 07:59:36", name = "CONF_LOGGED_TIME" });
            formdata.Add(new Field { Item = caseId, name = "CONF_CASE_ID" });

            baseRequest.data.formdata = formdata.ToArray();

            CreateResponse1 response;
            try
            {
                response = await _client.CreateAsync(baseRequest);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }

            return Ok(response);
        }


        [HttpPatch]
        public async Task<IActionResult> Update([FromQuery]string verintCaseId, [FromQuery]string confirmCaseId)
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

            //var baseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateRequest>(jsonCreateString);
            //baseRequest.caseid = caseId;

            //var formdata = baseRequest.data.formdata.ToList();
            //formdata.Add(new Field { Item = "18/06/2020 07:59:36", name = "CONF_LOGGED_TIME" });

            //baseRequest.data.formdata = formdata.ToArray();
            //baseRequest.completeSpecified = true;
            //baseRequest.complete = stringBoolean.Y;

            UpdateResponse1 response;
            try
            {
                //response = await _client.CreateAsync(baseRequest);
                response = await _client.UpdateAsync(new UpdateRequest
                {
                    caseid = verintCaseId,
                    @ref = confirmCaseId,
                    name = "confirm_integrationform",
                    currentpage = "3",
                    dataupdate = dataupdate.none
            });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

            return Ok(response);
        }


        [HttpPatch]
        [Route("/complete")]
        public async Task<IActionResult> Complete([FromQuery]string verintCaseId, [FromQuery]string confirmCaseId)
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

            //var baseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateRequest>(jsonCreateString);
            //baseRequest.caseid = caseId;

            //var formdata = baseRequest.data.formdata.ToList();
            //formdata.Add(new Field { Item = "18/06/2020 07:59:36", name = "CONF_LOGGED_TIME" });

            //baseRequest.data.formdata = formdata.ToArray();
            //baseRequest.completeSpecified = true;
            //baseRequest.complete = stringBoolean.Y;

            UpdateResponse1 response;
            try
            {
                //response = await _client.CreateAsync(baseRequest);
                response = await _client.UpdateAsync(new UpdateRequest
                {
                    caseid = verintCaseId,
                    @ref = confirmCaseId,
                    name = "confirm_integrationform",
                    dataupdate = dataupdate.none,
                    completeSpecified = true,
                    complete = stringBoolean.Y
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

            return Ok(response);
        }

        private const string jsonCreateString = @"
{
  ""name"": ""confirm_integrationform"",
  ""data"": {
    ""formdata"": [
        {
            ""name"": ""CONF_POC_CODE"",
            ""item"": ""WEB""
        },
        {
            ""name"": ""CONF_POC_NAME"",
            ""item"": ""Web/Online Form""
        },
        {
            ""name"": ""CONF_METH_CODE"",
            ""item"": ""WEB""
        },
        {
            ""name"": ""CONF_METH_NAME"",
            ""item"": ""Web/Online Form""
        },
        {
            ""name"": ""CONF_CUST_REF"",
            ""item"": ""101003283810""
        },
        {
            ""name"": ""CONF_CUST_TITLE"",
            ""item"": ""Mr""
        },
        {
            ""name"": ""CONF_CUST_SURNAME"",
            ""item"": ""Humphreys""
        },
        {
            ""name"": ""CONF_CUST_FORENAME"",
            ""item"": ""Elliott""
        },
        {
            ""name"": ""CONF_CUST_PHONE"",
            ""item"": ""07388909179""
        },
        {
            ""name"": ""CONF_CUST_EMAIL"",
            ""item"": ""elliott.humphreys@stockport.gov.uk""
        },
        {
            ""name"": ""CONF_CONTACT"",
            ""item"": ""Mr Elliott Elliott Humphreys""
        },
        {
            ""name"": ""CONF_CONTACT_PHONE"",
            ""item"": ""07388909179""
        },
        {
            ""name"": ""CONF_CONTACT_EMAIL"",
            ""item"": ""elliott.humphreys@stockport.gov.uk""
        },
        {
            ""name"": ""CONF_CUST_BUILDING"",
            ""item"": ""STOCKPORT DELIVERY OFFICE 1""
        },
        {
            ""name"": ""CONF_CUST_STREET"",
            ""item"": ""EXCHANGE STREET""
        },
        {
            ""name"": ""CONF_CUST_LOCALITY"",
            ""item"": """"
        },
        {
            ""name"": ""CONF_CUST_TOWN"",
            ""item"": ""STOCKPORT""
        },
        {
            ""name"": ""CONF_CUST_POSTCODE"",
            ""item"": ""SK1 1AA""
        },
        {
            ""name"": ""CONF_SERVICE_CODE"",
            ""item"": ""GREN      ""
        },
        {
            ""name"": ""CONF_SUBJECT_CODE"",
            ""item"": ""TTPO      ""
        },
        {
            ""name"": ""CONF_CLASSIFICATION"",
            ""item"": ""public_realm-greenspace-trees_request_new_tpo""
        },
        {
            ""name"": ""CboClassCode"",
            ""item"": ""REQU""
        },
        {
            ""name"": ""FOLLOW_UP_BY"",
            ""item"": ""10 Working Days""
        },
        {
            ""name"": ""CONF_DESC"",
            ""item"": ""(Lagan) Event Name: Request for a new Tree preservation order. \r\nEvent Name: Request for a new Tree preservation order. \r\nReason for the TPO request: test 2. \r\nFurther Location Information: test 1. \r\n""
        },
        {
            ""name"": ""CONF_LOCATION"",
            ""item"": ""test 1""
        },
        {
            ""name"": ""CONF_SITE_CODE"",
            ""item"": ""38102548""
        },
        {
            ""name"": ""CONF_SITE_NAME"",
            ""item"": ""HIBBERT LANE""
        },
        {
            ""name"": ""CONF_SITE_LOCALITY"",
            ""item"": ""MARPLE""
        },
        {
            ""name"": ""CONF_SITE_TOWN"",
            ""item"": ""STOCKPORT""
        },
        {
            ""name"": ""EFORM_UPDATED"",
            ""item"": ""true""
        },
        {
            ""name"": ""VIEWMODE"",
            ""item"": ""C""
        },
        {
            ""name"": ""CboCustomerTypeCode"",
            ""item"": ""PUBL""
        },
        {
            ""name"": ""CONF_LOGGED_BY"",
            ""item"": ""Lagan""
        }
    ]
  }
}";
    }
}