﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton.Model.Modpack
{
    public class CompiledMod
    {
        [JsonProperty("archives")]
        public List<InstallPlan> InstallPlans { get; set; }

        [JsonProperty("raw_ini")]
        public string RawINI { get; set; }

        [JsonProperty("is_separator")]
        public bool IsSeparator { get; set; }
    }

    public class InstallPlan
    {
        [JsonProperty("source_archive")]
        public InstallSourceArchive SourceArchive { get; set; }

        [JsonProperty("parings")]
        public List<FilePairing> FilePairings { get; set; }
    }

    public class FilePairing
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }

    public class InstallSourceArchive
    {
        [JsonProperty("archive_name")]
        public string ArchiveName { get; set; }

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

        [JsonProperty("direct_url")]
        public string DirectURL { get; set; }
    }
}
