using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;

namespace NupkgDownloader.Web.Areas
{
    public class BaseController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.User = await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);
            base.OnActionExecuting(context);
        }
    }
}