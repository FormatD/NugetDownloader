using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NupkgDownloader.Web.Areas.Template.Controllers
{
    [Area("Template")]
    public class JieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}