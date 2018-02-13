using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using brechtbaekelandt.ldap.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace brechtbaekelandt.ldap.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
