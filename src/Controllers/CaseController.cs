using System;
using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using verint_service.Services;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class CaseController : ControllerBase
    {
        private readonly ICaseService _caseService;

        public CaseController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]string caseId)
        {
            var verintCase = _caseService.GetCase(caseId);

            return Ok(verintCase);
        }

        [HttpPut]
        public IActionResult Update()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult Create()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery]string caseId)
        {
            throw new NotImplementedException();
        }
    }
}