using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace COLLECTION_MANAGEMENT_API.Filters
{
    public class ValidationFailedResult : ObjectResult
    {
        private HttpContext httpContext;

        public ValidationFailedResult(ModelStateDictionary modelState, HttpContext httpContext)
            : base(new ValidationResultModel(modelState, httpContext))
        {
            StatusCode = StatusCodes.Status200OK;
        }
    }
}
