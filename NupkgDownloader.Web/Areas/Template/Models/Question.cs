using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Areas.Template.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string Tag { get; set; }

        public bool IsTop { get; set; }

        public bool IsExcellect { get; set; }

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public User Creator { get; set; }
        public bool IsFinished { get; internal set; }
    }

    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public bool IsVip { get; internal set; }
        public int VipLevel { get; internal set; }
    }
}
