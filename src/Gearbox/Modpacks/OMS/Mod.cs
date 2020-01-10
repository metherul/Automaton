using System;
using System.Collections.Generic;
using System.IO;
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
                var installEntries = new List<InstallEntry>();

                foreach (var installSet in _modBase.Install)
                {
                    var tempIndex = installSet.Source.IndexOf(']');
                    var sourceIndex = installSet.Source[1..tempIndex];
                    
                    installEntries.Add(new InstallEntry()
                    {
                        Source = Sources[Convert.ToInt32(sourceIndex)],
                        From = installSet.Source[tempIndex..],
                        To = installSet.Target
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
    }
}