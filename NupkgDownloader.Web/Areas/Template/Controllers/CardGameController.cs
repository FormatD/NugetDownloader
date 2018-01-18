using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NupkgDownloader.Web.Areas;
using NupkgDownloader.Web.Areas.Template.Models;
using Microsoft.AspNetCore.Authorization;

namespace NupkgDownloader.Web.Controllers
{
    [Area("Template")]

    public class CardGameController : BaseController
    {
        private ApplicationDbContext _applicationDbContext;

        public CardGameController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : base(userManager)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/api/learn/latest")]
        [Authorize]
        public LearnRecord Lastest()
        {
            var lastRecord = _applicationDbContext.LearnRecords
                .Where(x => x.UserId == ApplicationUser.Id && x.Date.Date != DateTime.Today)
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();

            var latestRecord = _applicationDbContext.LearnRecords
                .Where(x => x.UserId == ApplicationUser.Id && x.Date.Date == DateTime.Today)
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();

            if (lastRecord == null)
            {
                lastRecord = new LearnRecord
                {
                    Date = DateTime.Today,
                    StartIndex = -1,
                    EndIndex = -1,
                    Days = 0,
                };
            }
            if (latestRecord == null)
            {
                Finish(new LearnRecord
                {
                    UserId = lastRecord.UserId,
                    Date = DateTime.Today,
                    Days = lastRecord.Days + 1,
                    StartIndex = lastRecord.EndIndex + 1,
                    EndIndex = lastRecord.EndIndex + 10,
                });
            }

            return lastRecord;
        }

        [Route("/api/learn/finish")]
        [HttpPost]
        public void Finish(LearnRecord record)
        {
            var latest = _applicationDbContext.LearnRecords
                .Where(x => x.UserId == ApplicationUser.Id)
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();

            if (latest != null && latest.Date == DateTime.Today)
                return;

            record.Date = DateTime.Today;
            record.UserId = ApplicationUser.Id;

            if (latest != null)
            {
                record.Days = latest.Days + 1;
            }

            _applicationDbContext.LearnRecords.Add(record);
            _applicationDbContext.SaveChanges();
        }
    }
}