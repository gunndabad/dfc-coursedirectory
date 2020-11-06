using Dfc.CourseDirectory.WebV2.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Dfc.CourseDirectory.WebV2.Features.Contact
{
    [Route("contact")]
    [AllowDeactivatedProvider]
    public class ContactController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
