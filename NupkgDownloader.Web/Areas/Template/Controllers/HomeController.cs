using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NupkgDownloader.Web.Areas.Template.Models;
using Microsoft.AspNetCore.Identity;

namespace NupkgDownloader.Web.Areas.Template.Controllers
{
    [Area("Template")]
    public class HomeController : BaseController
    {
        public HomeController(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public IActionResult Index()
        {
            return View(Samples.Instance.HomeViewModel);
        }

    }
}