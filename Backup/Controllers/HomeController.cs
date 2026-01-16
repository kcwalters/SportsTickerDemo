using Microsoft.AspNetCore.Mvc;
namespace NflTickerDemo.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
