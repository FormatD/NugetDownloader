using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace NupkgDownloader.Web.Areas.Nupkg.Controllers
{
    public class IpController : Controller
    {
        //
        // GET: /Ip/

        public string Index()
        {
            try
            {
                return System.IO.File.ReadAllText(Path.Combine("ip.txt"));
            }
            catch (FileNotFoundException)
            {
                return "No Such File";
            }
        }


        public string Set(string ip)
        {
            System.IO.File.WriteAllText(Path.Combine("ip.txt"), string.Format("{0}：{1}", DateTime.Now, ip));
            return System.IO.File.ReadAllText(Path.Combine("ip.txt"));
        }

        private static void EnsureDirectory(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}