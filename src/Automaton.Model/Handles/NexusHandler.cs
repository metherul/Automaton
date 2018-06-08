using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Automaton.Model.Handles
{
    public class NexusHandler
    {
        public static HttpClient NexusLoginInstance { get; set; }

        private const string LoginUrl = "https://www.nexusmods.com/Sessions/?Login";
        private const string DownloadUrl = "https://www.nexusmods.com/skyrim/download/";

        private static int LastDownloadPercentage = 0;

        /// <summary>
        /// Attempts to log current HttpClient into the NexusMods servers. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Will return true if successful, false if not.</returns>
        public static bool AttemptNexusLogIn(string username, string password)
        {
            NexusLoginInstance = new HttpClient();
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
            });

            var loginResult = NexusLoginInstance.PostAsync(new Uri(LoginUrl), formContent).Result;
            var isLoggedIn = loginResult.Headers.ToList()[1].Key != "NexusLoginErrorMessage";

            return isLoggedIn;
        }

        /// <summary>
        /// Will search for a valid URL from a target NXM string.
        /// </summary>
        /// <param name="nxmString">The NXM protocol string captured by the piped server.</param>
        /// <returns></returns>
        private static string GetDownloadFileUrl(string nxmString)
        {
            var splitNxm = nxmString.Split('/');
            var downloadPage = DownloadUrl + splitNxm[splitNxm.Length - 1];
            var downloadPageHtml = GetDownloadPage(downloadPage, NexusLoginInstance);

            var htmlParser = new HtmlParser();
            var html = htmlParser.Parse(downloadPageHtml);

            var matchingElement = html.All.First(x => x.Id == "dl_link");
            var matchingElementValue = matchingElement.GetAttribute("value");

            return matchingElementValue;
        }

        /// <summary>
        /// Attempts to download a file from the NexusMods servers
        /// </summary>
        /// <param name="nxmString">The NXM protocol string captured by the piped server.</param>
        /// <param name="progress">A generic IProgress callback to notify the ViewModel.</param>
        /// <returns></returns>
        public static bool DownloadNexusModFile(string nxmString, IProgress<NexusDownloadUpdate> progress)
        {
            var downloadFileUrl = GetDownloadFileUrl(nxmString);
            var fileName = GetFileName(downloadFileUrl);

            // Notify the UI that a new element needs to be added
            var nexusDownloadUpdate = new NexusDownloadUpdate()
            {
                FileName = fileName,
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName),
                DownloadSize = "0",
                Downloaded = "0",
                DownloadPercentage = 0
            };

            progress.Report(nexusDownloadUpdate);

            using (var webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += (sender, e) =>
                {
                    DownloadProgressChanged(sender, e, nexusDownloadUpdate, progress);
                };

                webClient.DownloadFileTaskAsync(new Uri(downloadFileUrl), nexusDownloadUpdate.FilePath);
            }

            return false;
        }

        private static void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e, NexusDownloadUpdate nexusDownloadUpdate, IProgress<NexusDownloadUpdate> progress)
        {
            //if (LastDownloadPercentage < e.ProgressPercentage)
            //{
            //    LastDownloadPercentage = e.ProgressPercentage;

            //    nexusDownloadUpdate.DownloadPercentage = e.ProgressPercentage;
            //    nexusDownloadUpdate.Downloaded = e.BytesReceived.ToString();
            //    nexusDownloadUpdate.DownloadSize = e.TotalBytesToReceive.ToString();

            //    progress.Report(nexusDownloadUpdate);
            //}

            LastDownloadPercentage = e.ProgressPercentage;

            nexusDownloadUpdate.DownloadPercentage = e.ProgressPercentage;
            nexusDownloadUpdate.Downloaded = e.BytesReceived.ToString();
            nexusDownloadUpdate.DownloadSize = e.TotalBytesToReceive.ToString();

            progress.Report(nexusDownloadUpdate);
        }

        private static string GetDownloadPage(string downloadPage, HttpClient httpClient)
        {
            using (var response = httpClient.GetAsync(downloadPage).Result)
            {
                using (var content = response.Content)
                {
                    var result = content.ReadAsStringAsync();

                    return result.Result;
                }
            }
        }

        private static string GetFileName(string fileUrl)
        {
            var splitFileUrl = fileUrl.Split('/');
            var lastClump = splitFileUrl[splitFileUrl.Length - 1];

            var fileName = lastClump.Substring(0, lastClump.LastIndexOf("?"));

            return HttpUtility.UrlDecode(fileName);
        }
    }

    public class NexusDownloadUpdate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }

        public string DownloadSize { get; set; }
        public string Downloaded { get; set; }
        public int DownloadPercentage { get; set; }
    }
}
