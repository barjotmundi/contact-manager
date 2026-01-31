using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers
{
    public class SplashController : Controller
    {
        public IActionResult Index() => View();
    }
}
