using Baidu.Aip;
using Baidu.Aip.Speech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NupkgDownloader.Web.Areas.Default.Models
{
    public class SpeachService
    {
        private Tts _tts;

        public SpeachService()
        {
            var API_KEY = "Dt0fCS3trEBcki9s4j5GAwtn";
            var SECRET_KEY = "aF0bSCaxGpr9jFlo7vaPVM6CEb7UE1Dv";
            _tts = new Tts(API_KEY, SECRET_KEY)
            {
                DebugLog = true
            };
        }

        public string Speach(string textToSpeach)
        {
            var mp3File = Path.Combine("cached_voice", $"{GetSafeName(textToSpeach)}.mp3");

            if (!File.Exists(mp3File))
            {
                var folder = Path.GetDirectoryName(mp3File);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var res = _tts.Synthesis(textToSpeach);
                if (res.Success)
                {
                    using (var fs = File.Create(mp3File))
                    {
                        fs.Write(res.Data, 0, res.Data.Length);
                    }
                }
                return mp3File;
            }

            return mp3File;
        }

        private static string GetSafeName(string textToSpeach)
        {
            Path.GetInvalidFileNameChars()
                .ToList()
                .ForEach(x => textToSpeach = textToSpeach.Replace(x, '_'));
            return textToSpeach;
        }
    }
}
