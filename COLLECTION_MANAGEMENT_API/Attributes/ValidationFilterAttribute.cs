using COLLECTION_MANAGEMENT_API.Filters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace COLLECTION_MANAGEMENT_API.Attributes
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ValidationFailedResult(context.ModelState, context.HttpContext);
            }
        }
    }
}
