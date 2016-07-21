using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "BookStore Shop";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "BookStore Shop";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
