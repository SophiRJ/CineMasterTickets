using Microsoft.AspNetCore.Mvc;

namespace CinemaMasterTicketsMVC.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
