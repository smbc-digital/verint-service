using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace verint_service.Attributes
{
    public class DevelopmentOnlyAttribute : ApiExplorerSettingsAttribute, IActionFilter
    {
        private static string enviroment;

        public DevelopmentOnlyAttribute()
        {
            if (enviroment.Equals("prod"))
                IgnoreApi = true;
        }
        public DevelopmentOnlyAttribute(IWebHostEnvironment webHostEnvironment)
        {
            enviroment = webHostEnvironment.EnvironmentName;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (IgnoreApi)
                context.Result = new UnauthorizedObjectResult("");
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
