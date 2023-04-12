using Microsoft.AspNetCore.Mvc;

namespace MVC_Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
