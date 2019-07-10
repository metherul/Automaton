using Automaton.Common;
using Automaton.Common.Model;
using SevenZipExtractor;
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
        public Dictionary<string, string> Hashes { get; private set; }

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

            Hashes = HashDownloadsFolder();

            Info("Done Hashing {0} Downloads", Hashes.Count);


            foreach (var mod in InstallerData.Mods)
            {
                Info("Creating mod directory for {0}", mod.Name);
                Directory.CreateDirectory(Path.Combine(ModsFolder, mod.Name));
            }

            var required_archives = (from mod in InstallerData.Mods
                                     from archive in mod.InstallPlans
                                     select archive.SourceArchive)
                                     .Append(InstallerData.MasterDefinition.MO2Archive)
                                     .ToList();

            Info("Found {0} archives in this pack", required_archives.Count());
            List<SourceArchive> missing = FindMissingArchives(Hashes, required_archives);

            Info("Missing {0} archives for this pack", missing.Count);

            Info("Getting API key for nexus, please click accept in your web browser");

            NexusAPIKey = GetNexusAPIKey();

            missing.PMap(WorkQueue, DownloadArchive).ToList();

            Hashes = HashDownloadsFolder();
            missing = FindMissingArchives(Hashes, required_archives);
            if (missing.Count > 0)
            {
                Info("Aborting installation, missing {0} archives", missing.Count);
                foreach (var miss in missing)
                    Info("    {0}", miss.ArchiveName);
                return;
            }

            required_archives.PMap(WorkQueue, InstallMod).ToList();


        }

        private SourceArchive InstallMod(SourceArchive archive)
        {
            SetStatus("Installing: {0}", archive.ArchiveName);
            var parentMod = (from mod in InstallerData.Mods
                             from iplan in mod.InstallPlans
                             where iplan.SourceArchive == archive
                             select mod).FirstOrDefault();

            if (parentMod == null)
                // TODO: this is gross, fix it
                return InstallMO2Archive(archive);

            var installationDirectory = Path.Combine(ModsFolder, parentMod.Name);
            //var filePairings = _parentMod.InstallPlans.SelectMany(x => x.FilePairings);

            var plan = parentMod.InstallPlans.Where(p => p.SourceArchive.SHA256 == archive.SHA256).First();


            // Verify to ensure that the mod's installation directory exists
            if (!Directory.Exists(installationDirectory))
            {
                Directory.CreateDirectory(installationDirectory);
            }

            // Get a dictionary of all the files we need to copy indexed by their name in the archive
            var extract_files = plan.FilePairings.GroupBy(p => p.From).ToDictionary(p => p.Key);


            // Let's pre-create all the directories in the mod folder so we don't have to check
            // for missing folders during the install.
            var directories = (from entry in extract_files
                               from to in entry.Value
                               let full_path = Path.Combine(installationDirectory, to.To)
                               select Path.GetDirectoryName(full_path)).Distinct();

            Info("Preparing directories for {0}", archive.Name);

            foreach (var dir in directories)
                Directory.CreateDirectory(dir);

            Info("Extracting {0}", archive.Name);
            var archive_path = Hashes[archive.SHA256];
            using (var file = new ArchiveFile(archive_path))
            {
                file.Extract(entry => {
                    if (extract_files.ContainsKey(entry.FileName))
                    {
                        // We may need to copy the same file to multiple locations so extract to the first one, 
                        // we'll copy this file around later.
                        var to = extract_files[entry.FileName].First();
                        var path = Path.Combine(installationDirectory, to.To);
                        return File.OpenWrite(Path.Combine(installationDirectory, path));
                    }

                    return null;
                });
            }

            // Now that we've installed all the files, copy around any files that exist in more than one location
            // in the mod.
            Info("Copying duplicated files for {0}", archive.Name);
            foreach (var copy_group in extract_files.Select(e => e.Value).Where(es => es.Count() > 1))
            {
                var from = copy_group.First();
                foreach (var to in copy_group.Skip(1))
                {
                    File.Copy(Path.Combine(installationDirectory, from.To),
                              Path.Combine(installationDirectory, to.To));
                }
            }

            /*
            foreach (var to_patch in plan.FilePairings.Where(p => p.patch_id != null))
            {
                using (var patch_stream = new System.IO.MemoryStream())
                {
                    // Read in the patch data
                    Patches[to_patch.patch_id].Extract(patch_stream);
                    patch_stream.Seek(0, System.IO.SeekOrigin.Begin);

                    System.IO.MemoryStream old_data = new System.IO.MemoryStream();
                    var to_file = Path.Combine(installationDirectory, to_patch.To);
                    // Read in the unpatched file
                    using (var unpatched = File.OpenRead(to_file))
                    {
                        unpatched.CopyTo(old_data);
                        old_data.Seek(0, System.IO.SeekOrigin.Begin);
                    }

                    // Patch it
                    using (var out_stream = File.OpenWrite(to_file))
                    {
                        var ps = new PatchingStream(old_data, patch_stream, out_stream, patch_stream.Length);
                        ps.Patch();
                    }
                }

            }*/


            return archive;
        }

        private SourceArchive InstallMO2Archive(SourceArchive archive)
        {
            var archive_path = Hashes[archive.SHA256];
            using (var af = new ArchiveFile(archive_path))
                af.Extract(InstallFolder);
            return archive;
        }

        private static List<SourceArchive> FindMissingArchives(Dictionary<string, string> hashes, List<SourceArchive> required_archives)
        {
            return required_archives.Where(a => !hashes.ContainsKey(a.SHA256))
                                           .ToList();
        }

        private Dictionary<string, string> HashDownloadsFolder()
        {
            Info("Hashing existing downloads");
            return Directory.EnumerateFiles(DownloadsFolder)
                                  .Where(f => File.Exists(f))
                                  .Where(f => supported_archives.Contains(Path.GetExtension(f)))
                                  .PMap(WorkQueue, f =>
                                  {
                                      SetStatus("Hashing {0}", Path.GetFileName(f));
                                      return new { name = f, hash = Utils.FileSHA256(f, true) };
                                  })
                                  .GroupBy(f => f.hash)
                                  .ToDictionary(f => f.Key, f => f.First().name);
        }

        private SourceArchive DownloadArchive(SourceArchive archive)
        {
            if (archive.Repository == "Nexus")
                return DownloadNexusMod(archive);
            else if (archive.DirectURL != null)
                return DownloadDirectURL(archive);
            else
                Info("Don't know how to download from {0} for {1}", archive.Name, archive.Repository);


            return archive;
        }

        private SourceArchive DownloadDirectURL(SourceArchive archive)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "requestor");
            DownloadURL(archive, client, archive.DirectURL);

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

            DownloadURL(archive, client, url);

            return archive;
        }

        private void DownloadURL(SourceArchive archive, HttpClient client, string url)
        {
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
