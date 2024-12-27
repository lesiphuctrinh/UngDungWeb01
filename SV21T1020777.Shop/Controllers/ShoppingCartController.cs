using Microsoft.AspNetCore.Mvc;

namespace SV21T1020777.Shop.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
