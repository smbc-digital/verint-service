using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using verint_service.Services.Case;

namespace verint_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class WebHooksController : ControllerBase
    {        
        private readonly ICaseService _caseService;
        
        public WebHooksController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        [HttpGet]
        [Route("upload-cached-notes/{id}")]
        public async Task<IActionResult> UploadCachedNotes(string id)
        {
            await _caseService.WriteCachedNotes(id);

            return Ok();
        }
    }
}