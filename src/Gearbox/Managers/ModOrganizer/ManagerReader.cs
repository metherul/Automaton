using Gearbox.IO;
using IniParser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gearbox.Managers.ModOrganizer
{
    public class ManagerReader
    {
        private const string _managerName = "ModOrganizer.exe";
        private readonly string _managerPath;

        private string _managerDir;
        private string _modDir;
        
        public ManagerReader(string managerPath)
        {
            _managerDir = Path.GetDirectoryName(managerPath);
            _modDir = Path.Combine(_managerDir, "mods");
        }

        public async Task<List<string>> GetSourceDirs()
        {
            var metaInis =
                (await AsyncFs.GetDirectories(_modDir, "*", SearchOption.TopDirectoryOnly))
                .Select(x => Path.Combine(x, "meta.ini"))
                .Where(x => File.Exists(x));

            var sourceDirs = new List<string>();
            var reader = new FileIniDataParser();

            foreach (var metaIni in metaInis)
            {
                var data = reader.ReadFile(metaIni);
                var sourcePath = data["General"]["installationFile"] ?? string.Empty;

                if (File.Exists(sourcePath))
                {
                    sourceDirs.Add(Path.GetDirectoryName(sourcePath));
                }
            }

            return sourceDirs.Distinct().ToList();
        }
    }
}