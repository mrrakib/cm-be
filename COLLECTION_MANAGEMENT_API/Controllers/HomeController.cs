using Microsoft.AspNetCore.Mvc;

namespace COLLECTION_MANAGEMENT_API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
