using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TicTakToe.Attributs
{
    public class NoTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var features = httpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (features != null)
            {
                features.MaxRequestBodySize = null;
            }
            base.OnActionExecuting(context);
        }
    }
}