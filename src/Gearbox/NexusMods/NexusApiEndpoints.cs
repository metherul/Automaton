using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Modpacks.Base;
using LanguageExt;
using Pathoschild.FluentNexus.Models;

using static LanguageExt.Prelude;

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
        public async Task<Option<ModHashResult[]>> Md5Search(string md5, Domain domain)
        {
            var result = await _nexusClient.Mods.GetModsByFileHash(domain.ToNexusString(), md5);
            return result.Any() ? Some(result) : None;
        }

        /// <summary>
        /// Searches multiple domains for Md5 archive metadata.
        /// </summary>
        /// <param name="md5">The md5 string of the archive file.</param>
        /// <returns></returns>
        public async Task<Option<ModHashResult[]>> MultiDomainMd5Search(string md5)
        {
            var domains = Enum.GetValues(typeof(Domain)).Cast<Domain>().ToList();
            var results = new List<ModHashResult>();

            foreach (var domain in domains)
            {
                var result = await Md5Search(md5, domain);
                result.IfSome((f) => results.Append(f));
            }

            return results.Any() ? Some(results.ToArray()) : None;
        }

        /// <summary>
        /// Returns the download URL of the specified archive data.
        /// </summary>
        /// <returns></returns>
        public async Task<Option<ModFileDownloadLink[]>> GetDownloadPaths(Domain domain, int modId, int fileId)
        {
            var results = await _nexusClient.ModFiles.GetDownloadLinks(domain.ToNexusString(), modId, fileId);

            return results.Any() ? Some(results) : None;
        }

        /// <summary>
        /// Returns the download URL of the specified archive data.
        /// </summary>
        /// <returns></returns>
        public async Task<Option<ModFileDownloadLink[]>> GetDownloadPath(Domain domain, int modId, int fileId, string nexusId, int nexusTimeout)
        {
            var results = await _nexusClient.ModFiles.GetDownloadLinks(domain.ToNexusString(), modId, fileId, nexusId, nexusTimeout);

            return results.Any() ? Some(results) : None;
        }
    }
}