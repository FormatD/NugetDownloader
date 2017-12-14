using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Models
{
    public class SendViewModel
    {
        public bool IsSuccessed { get; set; }

        public string Message { get; set; }

        public string Id { get; set; }

        public string Version { get; set; }
    }
}
