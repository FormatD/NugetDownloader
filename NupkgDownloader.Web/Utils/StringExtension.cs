using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Utils
{
    public static class StringExtension
    {
        public static string RemoveSuffix(this string source, string suffix)
        {
            while (!string.IsNullOrWhiteSpace(source) && source.EndsWith(suffix))
            {
                source = source.Substring(0, source.Length - suffix.Length);
            }

            return source;
        }
    }
}
