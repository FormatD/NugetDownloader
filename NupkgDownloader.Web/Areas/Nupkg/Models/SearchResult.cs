using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Models
{
    public class SearchResultViewModel
    {
        public string Keyword { get; set; }

        public IEnumerable<PackageViewModel> Result { get; set; }
    }
}
