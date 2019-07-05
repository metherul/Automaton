using Automaton.Common;
using IniParser;
using Newtonsoft.Json;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hephaestus.Model
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SourceArchive
    {
        public string FullPath { get; set; }

        public string ArchiveName { get { return Path.GetFileName(FullPath); } }

        [JsonProperty("game_name")]
        public string GameName { get; set; }

        [JsonProperty("mod_id")]
        public string ModId { get; set; }

        [JsonProperty("file_id")]
        public string FileId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("mod_name")]
        public string ModName { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("file_category")]
        public string FileCategory { get; set; }

        [JsonProperty("repository")]
        public string Repository { get; set; }

        [JsonProperty("md5")]
        public string MD5 { get; set; }

        [JsonProperty("sha256")]
        public string SHA256 { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("archive_entries")]
        public List<ArchiveEntry> ArchiveEntries { get; set; }

        [JsonProperty("direct_url", NullValueHandling = NullValueHandling.Include)]
        public string DirectURL { get; set; }

        [JsonProperty("author_id")]
        public long AuthorId { get; set; }

        [JsonProperty("donate_urls")]
        public List<string> DonateURLs { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        
        public static string META_CACHE_EXTENTION = ".source_archive";

        public static SourceArchive FromFileName(PackBuilder pb, string file)
        {
            var meta_name = file + META_CACHE_EXTENTION;
            if (File.Exists(meta_name))
            {
                var result = Utils.LoadJson<SourceArchive>(meta_name);
                result.FullPath = file;
                return result;
            }

            var inst = new SourceArchive();
            inst.FullPath = file;
            inst.Analyze(pb);

            Utils.WriteJson(inst, meta_name);

            return inst;
        }

        private void Analyze(PackBuilder pack_builder)
        {
            Log.Info("Analyzing: {0}", ArchiveName);
            using (var archive_file = new ArchiveFile(FullPath))
            {
                var streams = new Dictionary<string, HashingStream>();
                archive_file.Extract(e =>
                {
                    if (e.IsFolder) return null;
                    if (streams.ContainsKey(e.FileName))
                        return streams[e.FileName];

                    var stream = new HashingStream(e.FileName);
                    streams.Add(e.FileName, stream);
                    return stream;
                });

                var main_archive_hash = new HashingStream(FullPath);
                using (var i = File.OpenRead(FullPath))
                    i.CopyTo(main_archive_hash);
                main_archive_hash.Dispose();

                MD5 = main_archive_hash.MD5Hash;
                SHA256 = main_archive_hash.SHA256Hash;
                Size = Utils.FileSize(FullPath);

                var contents = from stream in streams.Values
                               select new ArchiveEntry
                               {
                                   FileName = stream.Filename,
                                   MD5 = stream.MD5Hash,
                                   SHA256 = stream.SHA256Hash,
                                   Size = stream.Size.ToString()
                               };

                ArchiveEntries = contents.ToList();

                if (File.Exists(FullPath + ".meta"))
                {
                    var data = DynamicIniData.FromFile(FullPath + ".meta");
                    var general = data.General;
                    GameName = general.gameName;
                    ModId = general.modID;
                    FileId = general.fileID;
                    Name = general.name;
                    Description = general.Description;
                    ModName = general.modName;
                    Version = general.version;
                    FileCategory = general.FileCategory;
                    Category = general.Category;
                    Repository = general.Repository;
                }
                
                var results = pack_builder.NexusClient.MD5SearchWithFallback(pack_builder.DefaultGame, MD5);
                    
                if (results == null)
                {
                    Log.Warn("Failed to find archive {0} on the nexus please manually update the metadata cache", FullPath);
                    return;
                }

                var matches = results.Where(result => result.FileDetails.FileName == ArchiveName && result.FileDetails.CategoryId != 6)
                                        .ToList();

                if (matches.Count > 1) {
                    Log.Warn("Found Multiple Matches for {0} please manually update the metadata cache", FullPath);
                    return;
                }

                if (matches.Count == 0) {
                    Log.Warn("No matches found for {0} please manually update the metadata cache", FullPath);
                    return;
                }

                var match = matches.First();

                GameName = match.ModDescription.DomainName;
                ModId = match.ModDescription.ModId.ToString();
                FileId = match.FileDetails.FileId.ToString();
                Name = match.FileDetails.Name;
                ModName = match.ModDescription.Name;
                Description = match.ModDescription.Description;
                Category = match.ModDescription.CategoryId.ToString();
                Version = match.FileDetails.Version;
                Repository = "Nexus";
                AuthorId = match.ModDescription.User.MemberID;
                Author = match.ModDescription.Author;

                ParseDonationURLs();

            }
            Log.Info("Finished Analyzing {0}", ArchiveName);

        }


        private static Regex PatreonRegex = new Regex("(?<=\\[url ?=)https?\\://www\\.patreon\\.com/.+?(?=\\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex PayPalRegex = new Regex("(?<=\\[url ?=)https?\\://www\\.patreon\\.com/.+?(?=\\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void ParseDonationURLs()
        {
            if (Description == null || AuthorId == 0) return;
           
            DonateURLs = new List<string>();
            DonateURLs.Add(String.Format("https://www.nexusmods.com/Core/Libs/Common/Widgets/DonatePopUp?user={0}", AuthorId));
            DonateURLs.Add(String.Format("https://www.nexusmods.com/Core/Libs/Common/Widgets/PayPalPopUp?user={0}", AuthorId));

            foreach (Match match in PatreonRegex.Matches(Description))
            {
                    DonateURLs.Add(match.Value);
            }

            foreach (Match match in PayPalRegex.Matches(Description))
            {
                DonateURLs.Add(match.Value);
            }

            DonateURLs = DonateURLs.Distinct().ToList();

        }
    }

    public class ArchiveEntry
    {
        [JsonProperty("md5")]
        public string MD5 { get; set; }

        [JsonProperty("sha256")]
        public string SHA256 { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }
    }


}
