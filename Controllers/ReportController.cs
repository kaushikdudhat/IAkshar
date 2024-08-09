using Microsoft.AspNetCore.Mvc;

namespace iAkshar.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
