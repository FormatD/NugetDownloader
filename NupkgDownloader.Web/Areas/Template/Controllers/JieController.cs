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
    public class JieController : BaseController
    {
        private static readonly int _pageSize = 10;

        public JieController(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public IActionResult Index(int page = 1)
        {
            var questions = Samples.Instance.Questions.Skip(_pageSize * (page - 1)).Take(_pageSize);
            return View(new QuestionsListViewModel { Page = page, Questions = questions, ReletedQuestions = questions.Take(4) });
        }

        public IActionResult Page(int page = 1)
        {
            var questions = Samples.Instance.Questions.Skip(_pageSize * (page - 1)).Take(_pageSize);
            return View("Index", new QuestionsListViewModel { Page = page, Questions = questions, ReletedQuestions = questions.Take(4) });
        }


        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            return View(Samples.Instance.QuestionDetails.FirstOrDefault(x => x.Id == id));
        }
    }
}