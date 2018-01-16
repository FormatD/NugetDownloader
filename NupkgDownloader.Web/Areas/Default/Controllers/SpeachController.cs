using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NupkgDownloader.Web.Areas.Default.Models;

namespace NupkgDownloader.Web.Areas.Default.Controllers
{
    public class SpeachController : Controller
    {
        SpeachService speachService = new SpeachService();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Play(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return NotFound();
            var mp3File = speachService.Speach(text);
            return File(System.IO.File.ReadAllBytes(mp3File), "audio/mpeg", mp3File);
        }
    }
}