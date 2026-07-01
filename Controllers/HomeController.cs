using EnterBridgePOC.Models;
using EnterBridgePOC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EnterBridgePOC.Controllers
{
    public class HomeController(IOrderStore orderStore) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string role)
        {
            TempData["Role"] = role;
            HttpContext.Session.SetString("Role", role);
            return RedirectToAction(nameof(Dashboard));
        }

        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("Role") ?? TempData["Role"] as string ?? "User";
            ViewData["Role"] = role;

            if (role == "Foreman")
                ViewData["PendingApprovals"] = orderStore.GetAll()
                    .Count(o => o.Status == ApprovalStatus.Pending);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
