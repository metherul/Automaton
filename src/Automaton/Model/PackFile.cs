using Newtonsoft.Json;
using System.Collections.Generic;

namespace Automaton.Model
{
    public class ModPack
    {
        [JsonProperty("automaton_version")]
        public string AutomatonVersion { get; set; }
        [JsonProperty("pack_name")]
        public string PackName { get; set; }
        [JsonProperty("pack_author")]
        public string PackAuthor { get; set; }
        [JsonProperty("pack_version")]
        public string PackVersion { get; set; }
        [JsonProperty("source_url")]
        public string SourceUrl { get; set; }
        [JsonProperty("optional_installation")]
        public OptionalInstallation OptionalInstallation { get; set; }
        [JsonProperty("mods")]
        public List<Mod> Mods { get; set; }
    }

    public class OptionalInstallation
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("default_image")]
        public string DefaultImage { get; set; }
        [JsonProperty("default_description")]
        public string DefaultDescription { get; set; }
        [JsonProperty("groups")]
        public List<Group> Groups { get; set; }
    }

    public class Group
    {
        [JsonProperty("header")]
        public string Header { get; set; }
        [JsonProperty("elements")]
        public List<Element> Elements { get; set; }
    }

    public class Element
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("is_checked")]
        public bool? IsChecked { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("flags")]
        public List<Flag> Flags { get; set; }
    }

    public class Flag
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }
    }

    public class Mod
    {
        [JsonProperty("mod_name")]
        public string ModName { get; set; }
        [JsonProperty("file_name")]
        public string FileName { get; set; }
        [JsonProperty("file_size")]
        public string FileSize { get; set; }
        [JsonProperty("mod_link")]
        public string ModLink { get; set; }
        [JsonProperty("checksum")]
        public string CheckSum { get; set; }
        [JsonProperty("load_order")]
        public int? LoadOrder { get; set; }
        [JsonProperty("installations")]
        public List<Installation> Installations { get; set; }
    }

    public class Installation
    {
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("target")]
        public string Target { get; set; }
        [JsonProperty("conditionals")]
        public List<Conditional> Conditionals { get; set; }
        [JsonProperty("ignores")]
        public List<string> Ignores { get; set; }
    }

    public class Conditional
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}

