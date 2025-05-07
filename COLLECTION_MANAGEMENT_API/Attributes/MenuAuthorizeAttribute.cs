using COLLECTION_MANAGEMENT_API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace COLLECTION_MANAGEMENT_API.Attributes
{
    public class MenuAuthorizeAttribute : TypeFilterAttribute
    {
        public MenuAuthorizeAttribute(string menuPath) : base(typeof(MenuAuthorizationFilter))
        {
            Arguments = new object[] { menuPath };
        }
    }
}
