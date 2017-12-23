using NupkgDownloader.Web.Areas.Template.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Areas.Template.Models
{
    public class QuestionsListViewModel
    {
        public int Page { get; set; }

        public IEnumerable<Question> Questions { get; set; }

        public IEnumerable<Question> ReletedQuestions { get; set; }
    }
}
