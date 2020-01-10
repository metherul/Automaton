using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Modpacks.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gearbox.NexusMods
{
    public partial class NexusApi 
    {
        /// <summary>
        /// Searches the target domain for Md5 archive metadata.
        /// </summary>
        /// <param name="md5">The md5 string of the archive file.</param>
        /// <param name="domain">The domain to search.</param>
        /// <returns></returns>
        public async Task<ICollection<Md5SearchResult>> Md5Search(string md5, Domain domain)
        {
            var url = $"/v1/{domain.ToNexusString()}/mods/md5_search/{md5}.json";
            var response = await MakeGenericRequest(url);

            if (response == null)
            {
                return null;
            }

            var jsonReader = new JsonTextReader(new StringReader(response));
            var jsonParser = await JArray.ReadFromAsync(jsonReader);

            var resultObjects = jsonParser
                .Where(x => (bool) x["mod"]["available"])
                .Where(x => (int) x["file_details"]["category_id"] != 6)
                .Select(x => new Md5SearchResult()
                {
                    ArchiveName = (string)jsonParser["file_details"]["file_name"],
                    ModId = (long) jsonParser["mod"]["mod_id"],
                    FileId = (long) jsonParser["file_details"]["file_id"],
                    Author = (string) jsonParser["mod"]["author"],
                    Md5 = (string) jsonParser["file_details"]["md5"],
                })
                .AsParallel()
                .ToList();

            return resultObjects;
        }

        /// <summary>
        /// Searches multiple domains for Md5 archive metadata.
        /// </summary>
        /// <param name="md5">The md5 string of the archive file.</param>
        /// <returns></returns>
        public async Task<ICollection<Md5SearchResult>> MultiDomainMd5Search(string md5)
        {
            var domains = Enum.GetValues(typeof(Domain)).Cast<Domain>().ToList();

            foreach (var domain in domains)
            {
                var result = await Md5Search(md5, domain);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}