using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NupkgDownloader.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NupkgDownloader.Web.Utils;

namespace NupkgDownloader.Web.Areas.Nupkg.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        private readonly ILogger<HomeController> _logger;

        public HomeController(UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _logger = logger;

        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(PackageController.Index), nameof(PackageController).RemoveSuffix("Controller"));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<Guid> GetCurrentUserId()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            _logger.LogError($"user is {user}");
            return user?.Id ?? Guid.Empty; // No need to cast here because user.Id is already a Guid, and not a string
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
