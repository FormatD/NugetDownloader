using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Areas.Template.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Question> TopQuestions { get; set; }

        public IEnumerable<Question> MostViewedQuestions { get; set; }


    }
}
