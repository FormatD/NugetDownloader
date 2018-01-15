using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace NupkgDownloader.Web.Areas.Template.Controllers
{
    [Area("Template")]
    public class CaseController : BaseController
    {

        public CaseController(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}