using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Gearbox.Modpacks.OMS
{
    public class Pack : IPack
    {
        public string PackType => "OMS";
        public string Developer => "metherul";
        public string Version => "0.0.1";
        public IHeader Header { get; set; }
        public ITheme Theme { get; set; }
        public List<IMod> Mods { get; set; } = new List<IMod>();
        public List<ISource> Sources { get; set; } = new List<ISource>();

        public async Task FromFile(string filePath)
        {
            var packFile = ZipFile.OpenRead(filePath);
            var packEntries = packFile.Entries;

            var headerEntry = packEntries.First(x => x.FullName == "header.json");
            var header = new Header();
            await header.FromJson(headerEntry.Open()); 

            Header = header;

            foreach (var entry in packEntries) 
            {
                var parentDir = Path.GetDirectoryName(entry.FullName);

                if (parentDir == "mods")
                {
                    var mod = new Mod();
                    await mod.FromJson(entry.Open());
                    
                    Mods.Add(mod);
                }

                if (parentDir == "utilities")
                {
                    var mod = new Utility();
                    await mod.FromJson(entry.Open());

                    Mods.Add(mod);
                }

                if (parentDir == "archives")
                {
                    var source = new Source();
                    await source.FromJson(entry.Open());

                    Sources.Add(source);
                }
            }
        }
    }
}