using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LunarSelector
{
    internal class LMDownloader
    {
        internal const string LMFolder = "./LM/";


        private const string LMHistoryPage = "https://www.smwcentral.net/?p=section&a=versionhistory&id=4583";

        public string Version { get; private set; }

        internal string? DlLink { get; set; }

        public LMDownloader(string version)
        {
            Version = version;
            DlLink = null;
        }

        public async Task<bool> DownloadAsync()
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(DlLink);
            if (!response.IsSuccessStatusCode) { return false; }

            try
            {
                var zip = new ZipArchive(response.Content.ReadAsStream());

                foreach (var entry in zip.Entries)
                {
                    var extractedName = entry.Name switch
                    {
                        "Lunar Magic.chm" => $"Lunar Magic {Version}.chm",
                        "readme.txt" => $"readme {Version}.txt",
                        "Lunar Magic.exe" => $"LM{Version}.exe",
                        _ => null
                    };

                    if (extractedName != null)
                    {
                        entry.ExtractToFile(LMFolder + extractedName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return File.Exists($"{LMFolder}LM{Version}.exe");
        }

        public async Task<bool> ExistsAsync()
        {
            return DlLink != null || await GetDlLink();
        }

        private async Task<bool> GetDlLink()
        {
            if (DlLink == null)
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(LMHistoryPage);
                if (!response.IsSuccessStatusCode) { return false; }
                var content = await response.Content.ReadAsStringAsync();

                var regex = new Regex($@"&amp;id=(?<id>\d+)(&amp;r=0)?"">Lunar Magic {Version.Replace(".", "\\.")}");
                var match = regex.Match(content);

                if (!match.Success) { return false; }

                DlLink = $"https://dl.smwcentral.net/{match.Groups["id"].Value}/lm{Version.Replace(".", "")}.zip";
            }

            return DlLink != null;
        }
    }
}
