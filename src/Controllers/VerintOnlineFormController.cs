using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Verint;
using System.Collections.Generic;
using System.Threading.Tasks;
using verint_service.Services.VerintOnlineForm;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class VerintOnlineFormController : ControllerBase
    {
        private readonly IVerintOnlineFormService _verintOnlineFormService;

        public VerintOnlineFormController(IVerintOnlineFormService verintOnlineFormService)
        {
            _verintOnlineFormService = verintOnlineFormService;
        }

        /// <summary>
        /// Creates a Verint Online From, Verint Case, and triggers Confirm integration
        /// </summary>
        /// <remarks>
        /// Use the following object to create a TPO VOF case.
        ///     {
        ///        "verintCase": {
        ///            "eventCode": 2002573,
        ///            },
        ///        "formName": "confirm_integrationform",
        ///        "formData": {
        ///            "CONF_POC_CODE":"WEB",
        ///            "CONF_POC_NAME":"Web/Online Form",
        ///            "CONF_METH_CODE":"WEB",
        ///            "CONF_METH_NAME":"Web/Online Form",
        ///            "CONF_CUST_REF":"101003283810",
        ///            "CONF_CUST_TITLE":"Mr",
        ///            "CONF_CUST_SURNAME":"Humphreys",
        ///            "CONF_CUST_FORENAME":"Elliott",
        ///            "CONF_CUST_PHONE":"07388909179",
        ///            "CONF_CUST_EMAIL":"elliott.humphreys@stockport.gov.uk",
        ///            "CONF_CONTACT":"Mr Elliott Elliott Humphreys",
        ///            "CONF_CONTACT_PHONE":"07388909179",
        ///            "CONF_CONTACT_EMAIL":"elliott.humphreys@stockport.gov.uk",
        ///            "CONF_CUST_BUILDING":"STOCKPORT DELIVERY OFFICE 1",
        ///            "CONF_CUST_STREET":"EXCHANGE STREET",
        ///            "CONF_CUST_LOCALITY":"",
        ///            "CONF_CUST_TOWN":"STOCKPORT",
        ///            "CONF_CUST_POSTCODE":"SK1 1AA",
        ///            "CONF_SERVICE_CODE":"GREN",
        ///            "CONF_SUBJECT_CODE":"TTPO",
        ///            "CONF_CLASSIFICATION":"public_realm-greenspace-trees_request_new_tpo",
        ///            "CboClassCode":"REQU",
        ///            "FOLLOW_UP_BY":"10 Working Days",
        ///            "CONF_DESC":"(Lagan) Event Name: Request for a new Tree preservation order. \r\nEvent Name: Request for a new Tree preservation order. \r\nReason for the TPO //request:Further?/ Location Information: test 1. \r\n",
        ///            "CONF_LOCATION":"test 1",
        ///            "CONF_SITE_CODE":"38102548",
        ///            "CONF_SITE_NAME":"HIBBERT LANE",
        ///            "CONF_SITE_LOCALITY":"MARPLE",
        ///            "CONF_SITE_TOWN":"STOCKPORT",
        ///            "EFORM_UPDATED":"true",
        ///            "VIEWMODE":"C",
        ///            "CboCustomerTypeCode":"PUBL",
        ///            "CONF_LOGGED_BY":"Lagan",
        ///            "le_eventcode":"2002573",
        ///            "le_queue_complete":"AppsConfirmQueuePending",
        ///            "le_associated_obj_type":"D4",
        ///            "le_associated_obj_id":"1002109494",
        ///            "le_description":"tpo"
        ///        }
        ///    }
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create(VerintOnlineFormRequest model) => Ok(await _verintOnlineFormService.CreateVOFCase(model));


        /* this model will be moved to StockportGovUK...Models package*/
        public class VerintOnlineFormRequest
        {
            public Case VerintCase { get; set; }

            public string FormName { get; set; }

            public IDictionary<string, string> FormData { get; set; }
        }

        /* this model will be moved to StockportGovUK...Models package*/
        public class VerintOnlineFormResponse
        {
            public string VerintCaseReference { get; set; }
            public string VerintOnlineFormReference { get; set; }
        }
    }
}
