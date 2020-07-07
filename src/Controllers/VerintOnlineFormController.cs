using Microsoft.AspNetCore.Mvc;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
using System.Threading.Tasks;
using verint_service.Attributes;
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
        [HttpPost]
        public async Task<IActionResult> Create(VerintOnlineFormRequest model)
            => Ok(await _verintOnlineFormService.CreateVOFCase(model));

        /// <summary>
        /// Gets a verint online form case
        /// </summary>
        [HttpGet]
        [DevelopmentOnly]
        public async Task<IActionResult> GetCase(string verintOnlineFormReference)
            => Ok(await _verintOnlineFormService.GetVOFCase(verintOnlineFormReference));
    }
}
