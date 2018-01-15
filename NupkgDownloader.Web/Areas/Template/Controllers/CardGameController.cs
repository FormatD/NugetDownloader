using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NupkgDownloader.Web.Areas;

namespace NupkgDownloader.Web.Controllers
{
    [Area("Template")]
    public class CardGameController : BaseController
    {

        public CardGameController(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}