using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gearbox.Shared.ModOrganizer
{
    public class IniReader
    {
        private readonly string _iniFile;

        public IniReader(string iniFile)
        {
            _iniFile = iniFile;
        }

        public async Task<List<string>> GetRecentDirs()
        {
            var fileContents = (await File.ReadAllLinesAsync(_iniFile)).ToList();
            var headerIndex = fileContents.IndexOf("[recentDirectories]");
            var recentDirs = new List<string>();

            for (var i = headerIndex + 1; i < fileContents.Count; i++)
            {
                var line = fileContents[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                if (line.Length < 11)
                {
                    continue;
                }

                if (line[2..11] != "directory=")
                {
                    continue;
                }

                var directory = Path.GetFullPath(line[12..]);

                if (!Directory.Exists(directory))
                {
                    continue;
                }

                recentDirs.Add(directory);
            }

            return recentDirs;
        }
    }
}
