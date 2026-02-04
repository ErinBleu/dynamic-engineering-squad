using Microsoft.AspNetCore.Mvc;

namespace InfrastructureApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
