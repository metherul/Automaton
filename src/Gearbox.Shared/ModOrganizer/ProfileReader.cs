using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gearbox.Shared.ModOrganizer
{
    public class ProfileReader
    {
        private readonly string _profilePath;

        public ProfileReader(string profilePath)
        {
            _profilePath = profilePath;
        }

        /// <summary>
        /// Returns the mod list specific for the profile
        /// Mods are in the same order as in modlist.txt.
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetModList(ModStatus modStatus = ModStatus.EnabledOnly)
        {
            var modList = Path.Combine(_profilePath, "modlist.txt");
            var prefix = modStatus switch
            {
                ModStatus.Any => "",
                ModStatus.EnabledOnly => "+",
                _ => "+"
            };

            return (await File.ReadAllLinesAsync(modList))
                .Where(x => !x.StartsWith("#") && x.StartsWith(prefix))
                .Select(x => x[1..])
                .ToList();
        }
    }

    public enum ModStatus
    {
        EnabledOnly,
        Any
    }
}
