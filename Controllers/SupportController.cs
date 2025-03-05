using Microsoft.AspNetCore.Mvc;

namespace TourFlow.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult Support()
        {
            return View();
        }
    }
}
