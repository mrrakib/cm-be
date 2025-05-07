using COLLECTION_MANAGEMENT_API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace COLLECTION_MANAGEMENT_API.Attributes
{
    public class MenuAuthorizeAttribute : TypeFilterAttribute, IOrderedFilter
    {
        public int Order { get; set; }
        public MenuAuthorizeAttribute(string menuPath) : base(typeof(MenuAuthorizationFilter))
        {
            Arguments = new object[] { menuPath };
        }
    }
}
