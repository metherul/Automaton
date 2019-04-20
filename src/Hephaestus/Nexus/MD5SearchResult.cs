using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hephaestus.Nexus
{
    public class MD5SearchResult
    {
        [JsonProperty("mod")]
        public ModDescription ModDescription { get; set;}
        
        [JsonProperty("file_details")]
        public FileDetails FileDetails { get; set; }
    }

    public class ModDescription
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        [JsonProperty("mod_id")]
        public long ModId { get; set; }

        [JsonProperty("game_id")]
        public long GameId { get; set; }

        [JsonProperty("domain_name")]
        public string DomainName { get; set; }

        [JsonProperty("category_id")]
        public long CategoryId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("user")]
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        [JsonProperty("member_id")]
        public long MemberID { get; set; }
   
        [JsonProperty("member_group_id")]
        public long MemberGroupID { get; set; }

        [JsonProperty("name")]
        public string Name;
    }

    public class FileDetails
    {
        [JsonProperty("file_id")]
        public long FileId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("category_id")]
        public long CategoryId { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("md5")]
        public string MD5 { get; set; }
    }
}
