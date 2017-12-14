using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NupkgDownloader.Web.Areas.Template.Controllers
{
    [Area("Template")]
    public class CaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}