using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NupkgDownloader.Web.Areas.Template.Models;

namespace NupkgDownloader.Web.Areas.Template.Controllers
{
    [Area("Template")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(Samples.Instance.HomeViewModel);
        }

    }
}