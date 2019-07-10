using Automaton.Common;
using Automaton.Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace Automaton.Winforms
{
    public class ProgramLogic
    {
        private Action<string> _logfn;
        private Action<string[]> _updateWorkersFn;

        public InstallerData InstallerData {get; private set; }
        public WorkQueue WorkQueue { get; }
        public string InstallFolder { get; internal set; }

        public bool Installing { get; private set; }

        public string ModsFolder
        {
            get
            {
                return Path.Combine(InstallFolder, "mods");
            }
        }

        public string DownloadsFolder
        {
            get
            {
                return Path.Combine(InstallFolder, "downloads");
            }
        }

        public string NexusAPIKey { get; private set; }

        internal void SetWorkerUpdateFn(Action<string[]> updateWorkers)
        {
            _updateWorkersFn = updateWorkers;
        }

        public ProgramLogic()
        {
            InstallerData = new InstallerData();
            WorkQueue = new WorkQueue(this);
            StartWorkerUpdater();
        }

        private void StartWorkerUpdater()
        {
            new Thread(() =>
            {
                try
                {
                    while (WorkQueue.Running)
                    {
                        string[] statuses = new string[WorkQueue.WorkerStatus.Length];
                        for (var idx = 0; idx < WorkQueue.WorkerStatus.Length; idx++)
                            statuses[idx] = String.Format("CPU {0} - {1}", idx, WorkQueue.WorkerStatus[idx]);

                        if (_updateWorkersFn != null)
                            _updateWorkersFn(statuses);

                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {

                }
            }).Start();
        }

        public void Info(string fmt, params object[] args)
        {
            var str = String.Format(fmt, args.Select(arg => arg.ToString()).ToArray()) + "\n";
            _logfn(str);
        }

        public void SetStatus(string fmt, params object[] args)
        {
            var str = String.Format(fmt, args.Select(arg => arg.ToString()).ToArray()) + "\n";
            WorkQueue.SetWorkerStatus(str);
        }

        public void SetLogFn(Action<string> logLine)
        {
            this._logfn = logLine;
        }






        public void LoadModPack(string filename)
        {
            Info("Loading Mod Pack: {0}", filename);
            using (var fs = File.OpenRead(filename))
            using (var zipfile = new ZipArchive(fs))
            {
                var entry = zipfile.GetEntry("pack.auto_definition");

                MasterDefinition master;
                using (var entry_stream = entry.Open())
                {
                    master = Utils.LoadJson<MasterDefinition>(entry_stream);
                }
                InstallerData.MasterDefinition = master;

                Info("Loaded master definition for: {0} by {1}", master.PackName, master.AuthorName);

                var mods = zipfile.Entries
                                   .Where(e => e.FullName.StartsWith("mods\\"))
                                   .Select(e => e.FullName.Split('\\').Skip(1).First())
                                   .ToHashSet();

                var mod_defs = mods.PMap(WorkQueue, mod_name =>
                {
                    using (var fs_inner = File.OpenRead(filename))
                    {
                        using (var zipfile_inner = new ZipArchive(fs_inner))
                        {
                            SetStatus("Loading Mod Data for: {0}", mod_name);
                            var path = Path.Combine("mods", mod_name, "install.json");
                            var def_entry = zipfile_inner.GetEntry(path);

                            path = Path.Combine("mods", mod_name, "meta.ini");
                            var ini_entry = zipfile_inner.GetEntry(path);
                            Mod mod;
                            using (var s = def_entry.Open())
                                mod = Utils.LoadJson<Mod>(s);

                            using (var s = def_entry.Open())
                                mod.ModIni = Utils.Slurp(s);

                            return mod;
                        }
                    }
                }).ToList();

                InstallerData.Mods = mod_defs;

                Info("Loaded information for {0} mods", mod_defs.Count());
                

            }
        }

        private HashSet<string> supported_archives = new HashSet<string>() { ".7z", ".7zip", ".rar", ".zip" };

        public void Install()
        {
            Installing = true;
            Info("Starting Installation of {0}", InstallerData.MasterDefinition.PackName);

            Directory.CreateDirectory(InstallFolder);
            Directory.CreateDirectory(ModsFolder);
            Directory.CreateDirectory(DownloadsFolder);

            Info("Hashing existing downloads");

            var hashes = Directory.EnumerateFiles(DownloadsFolder)
                                  .Where(f => File.Exists(f))
                                  .Where(f => supported_archives.Contains(Path.GetExtension(f)))
                                  .PMap(WorkQueue, f =>
                                  {
                                      SetStatus("Hashing {0}", Path.GetFileName(f));
                                      return new { name = f, hash = Utils.FileSHA256(f, true) };
                                  })
                                  .GroupBy(f => f.hash)
                                  .ToDictionary(f => f.Key, f => f.First().hash);

            Info("Done Hashing {0} Downloads", hashes.Count);


            foreach (var mod in InstallerData.Mods)
            {
                Info("Creating mod directory for {0}", mod.Name);
                Directory.CreateDirectory(Path.Combine(ModsFolder, mod.Name));
            }

            var required_archives = (from mod in InstallerData.Mods
                                     from archive in mod.InstallPlans
                                     select archive.SourceArchive).ToList();

            Info("Found {0} archives in this pack", required_archives.Count());

            var missing = required_archives.Where(a => !hashes.ContainsKey(a.SHA256))
                                           .ToList();

            Info("Missing {0} archives for this pack", missing.Count);

            Info("Getting API key for nexus, please click accept in your web browser");

            NexusAPIKey = GetNexusAPIKey();

            missing.PMap(WorkQueue, DownloadArchive).ToList();

        }

        private SourceArchive DownloadArchive(SourceArchive archive)
        {
            if (archive.Repository == "Nexus")
                return DownloadNexusMod(archive);
            else
                Info("Don't know how to download from {0} for {1}", archive.Name, archive.Repository);


            return archive;
        }

        private SourceArchive DownloadNexusMod(SourceArchive archive)
        {
            SetStatus("Getting download link for {0}", archive.ArchiveName);
            var client = BaseNexusClient();
            string url;
            string get_url_link = String.Format("https://api.nexusmods.com/v1/games/{0}/mods/{1}/files/{2}/download_link.json", 
                                                ConvertGameName(archive.GameName), archive.ModId, archive.FileId);
            using (var s = client.GetStreamSync(get_url_link))
            {
                url = Utils.LoadJson<List<DownloadLink>>(s).First().URI;
            }

            SetStatus("Downloading {0}", archive.ArchiveName);

            long total_read = 0;
            int buffer_size = 1024 * 32;

            var response = client.GetSync(url);
            var stream = response.Content.ReadAsStreamAsync();
            stream.Wait();

            var header = response.Content.Headers.GetValues("Content-Length").FirstOrDefault();
            double content_size = header != null ? Double.Parse(header) : 1;

            var output_path = Path.Combine(DownloadsFolder, archive.ArchiveName);

            using (var webs = stream.Result)
            using (var fs = File.OpenWrite(output_path))
            {
                var buffer = new byte[buffer_size];
                while (true)
                {
                    var read = webs.Read(buffer, 0, buffer_size);
                    if (read == 0) break;
                    SetStatus("{0:00.00}% downloading {1}", (total_read / content_size * 100.0), archive.Name);

                    fs.Write(buffer, 0, read);
                    total_read += read;
                  
                }
            }
            SetStatus("Hashing {0}", archive.Name);
            Utils.FileSHA256(output_path, true);

            return archive;
        }

        private HttpClient BaseNexusClient()
        {
            var _baseHttpClient = new HttpClient();

            var platformType = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            var headerString = $"Automaton/{Assembly.GetEntryAssembly().GetName().Version} ({Environment.OSVersion.VersionString}; {platformType}) {RuntimeInformation.FrameworkDescription}";

            _baseHttpClient.DefaultRequestHeaders.Add("User-Agent", headerString);
            _baseHttpClient.DefaultRequestHeaders.Add("apikey", NexusAPIKey);
            _baseHttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _baseHttpClient.DefaultRequestHeaders.Add("Application-Name", "Automaton");
            _baseHttpClient.DefaultRequestHeaders.Add("Application-Version", $"{Assembly.GetEntryAssembly().GetName().Version}");
            return _baseHttpClient;
        }

        private object ConvertGameName(string gameName)
        {
            if (gameName == "SkyrimSE") return "skyrimspecialedition";
            return gameName;
                
        }

        public string GetNexusAPIKey()
        {
            FileInfo fi = new FileInfo("nexus.key_cache");
            if (fi.Exists && fi.LastWriteTime > DateTime.Now.AddHours(-12))
            {
                return Utils.Slurp("nexus.key_cache");
            }

            var guid = Guid.NewGuid();
            var _websocket = new WebSocket("wss://sso.nexusmods.com")
            {
                SslConfiguration =
            {
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
            }
            };

            TaskCompletionSource<string> api_key = new TaskCompletionSource<string>();
            _websocket.OnMessage += (sender, msg) =>
            {
                api_key.SetResult(msg.Data);
                return;
            };

            _websocket.Connect();
            _websocket.Send("{\"id\": \"" + guid + "\", \"appid\": \"Automaton\"}");

            Process.Start($"https://www.nexusmods.com/sso?id={guid}&application=Automaton");

            api_key.Task.Wait();
            var result = api_key.Task.Result;
            File.WriteAllText("nexus.key_cache", result);
            return result;
        }
    }
}
