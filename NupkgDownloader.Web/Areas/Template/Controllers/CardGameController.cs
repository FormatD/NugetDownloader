using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NupkgDownloader.Web.Controllers
{
    [Area("Template")]
    public class CardGameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}