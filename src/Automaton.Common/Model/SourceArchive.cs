﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Automaton.Common.Model
{
    public class SourceArchive 
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

        [JsonProperty("author_id")]
        public long AuthorId { get; set; }

        [JsonProperty("donate_urls")]
        public List<string> DonateURLs { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

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
