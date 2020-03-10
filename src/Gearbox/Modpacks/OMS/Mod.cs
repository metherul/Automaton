using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.IO;
using Gearbox.Modpacks.OMS.Base;

namespace Gearbox.Modpacks.OMS
{
    public class Mod : IMod, IJsonSource 
    {
        public string Name => _modBase?.Name;
        public string[] Sources => _modBase?.RequiredArchives;

        private IReadOnlyList<InstallEntry> _installEntries;
        public IReadOnlyList<InstallEntry> InstallEntries
        {
            get
            {
                if (_installEntries != null)
                {
                    return _installEntries;
                }
                
                var metaPack = PackResources.Pack;

                var sources = metaPack.Sources;
                var installEntries = new List<InstallEntry>();

                foreach (var installSet in _modBase.Install)
                {
                    var tempIndex = installSet.Source.IndexOf(']');
                    var sourceIndex = Convert.ToInt32(installSet.Source[1..tempIndex]);

                    installEntries.Add(new InstallEntry()
                    {
                        Source = sources.FirstOrDefault(x => x.Name == Sources[sourceIndex]),
                        From = installSet.Source[(tempIndex + 2)..],
                        To = installSet.Target[1..]
                    });
                }

                return installEntries;
            }
        }

        private ModBase _modBase;
        
        public async Task FromJson(Stream stream)
        {
            _modBase = await JsonUtils.ReadJson<ModBase>(stream);
        }

        public async Task GetInstallCommands(List<ISource> sources)
        {
            
        }
    }
}