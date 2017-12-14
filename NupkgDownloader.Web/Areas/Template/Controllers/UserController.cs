using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NupkgDownloader.Web.Areas.Template.Controllers
{
    [Area("Template")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Activate()
        {
            return View();
        }

        public IActionResult Forget()
        {
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Message()
        {
            return View();
        }

        public IActionResult Reg()
        {
            return View();
        }

        public IActionResult Set()
        {
            return View();
        }
    }
}