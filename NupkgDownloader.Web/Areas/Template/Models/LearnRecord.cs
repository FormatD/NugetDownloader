using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Areas.Template.Models
{
    public class LearnRecord
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime Date { get; set; }

        public int Days { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }
    }
}
