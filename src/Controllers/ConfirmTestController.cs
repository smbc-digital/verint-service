using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Services.Case;
using VOFWebService;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication(IgnoredRoutes = new[] { "/api/v1/case/event" })]
    public class ConfirmTestController : ControllerBase
    {
        private readonly ICaseService _caseService;
        private readonly IEndpointBehavior _requestBehavior;

        public ConfirmTestController(
            ICaseService caseService,
            IEndpointBehavior requestBehavior)
        {
            _caseService = caseService;
            _requestBehavior = requestBehavior;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string caseId) => Ok(await GetCase(caseId));

        /**
         * the prefered way
         */
        #region Create

        #endregion

        /**
         * the way that should work if there wasn't bugs with Update
         */
        #region Create-Update-Flow
        [HttpPost]
        [Route("create-get-update")]
        public async Task<IActionResult> CreateGetUpdate()
        {
            var createResponse = await CreateBasic(); // should check was successful

            /**
             * this should return the Verint Case ID - however it does, once the update is performed you can see the Verint Case ID 
             * this could be a bug - or inteneded functionality
             * either way it makes this method of making the case in one call with no interaction from a person redundant
             */
            var getCaseResponse = await GetCase(createResponse.CreateResponse.@ref); // should check was successful

            var updateResponse = await UpdateBasic(getCaseResponse.GetResponse.caseid, createResponse.CreateResponse.@ref); // should check was successful

            return Ok(new
            {
                CaseId = getCaseResponse.GetResponse.caseid,
                ConfirmId = createResponse.CreateResponse.@ref,
                getCaseResponse,
                createResponse
            });
        }

        private async Task<CreateResponse1> CreateBasic()
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

            return await _client.CreateAsync(new CreateRequest
                {
                    name = "confirm_integrationform",
                    data = new Data
                    {
                        formdata = requiredFields
                    }
                });
        }

        private async Task<UpdateResponse1> UpdateBasic(string verintCaseId, string confirmCaseId)
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

            var updateRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateRequest>(jsonCreateString);

            updateRequest.caseid = verintCaseId;
            updateRequest.@ref = confirmCaseId;
            updateRequest.completeSpecified = true;
            updateRequest.complete = stringBoolean.Y;
            updateRequest.dataupdate = dataupdate.overwrite;

            var formdata = updateRequest.data.formdata.ToList();
            formdata.Add(new Field { Item = "18/06/2020 07:59:36", name = "CONF_LOGGED_TIME" });

            updateRequest.data.formdata = formdata.ToArray();

            return await _client.UpdateAsync(updateRequest);
        }
        #endregion

        /**
         * the working work around 
         */
        #region CreateVerintCase-CreateVOF-Update-Flow
        [HttpPost]
        [Route("verint-create-vof-create-update")]
        public async Task<IActionResult> CreateVerintCaseCreateVOFUpdate()
        {
            var createVerintCaseResponse = await _caseService.CreateCase(new Case
            {
                EventCode = 2002573
            }); // should check was successful

            var createResponse = await CreateFromBaseVerintCase(createVerintCaseResponse); // should check was successful

            var updateResponse = await UpdateBasic(createVerintCaseResponse, createResponse.CreateResponse.@ref); // should check was successful

            return Ok(new
            {
                createVerintCaseResponse,
                createResponse,
                updateResponse
            });
        }

        private async Task<CreateResponse1> CreateFromBaseVerintCase(string verintCaseId)
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

            return await _client.CreateAsync(new CreateRequest
            {
                name = "confirm_integrationform",
                caseid = verintCaseId,
                data = new Data
                {
                    formdata = requiredFields
                }
            });
        }
        #endregion

        private async Task<GetResponse1> GetCase(string caseId)
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

            return await _client.GetAsync(new GetRequest
            {
                @ref = caseId
            });
        }

        // minimum required fields for a VOF case to be created
        private Field[] requiredFields = new Field[]
        {
            new Field
            {
                name = "le_eventcode",
                Item = "2002573"
            }
        };

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
        ""item"": ""GREN""
    },
    {
        ""name"": ""CONF_SUBJECT_CODE"",
        ""item"": ""TTPO""
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
    },
    {
        ""name"": ""le_eventcode"",
        ""item"": ""2002573""
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
    }
]
  }
}";
    }
}