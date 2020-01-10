using System;
using System.Threading.Tasks;
using Gearbox.Modpacks;

namespace Gearbox.Managers.ModOrganizer
{
    public class Manager : IManager
    {
        private const string Version =
            @"https://github.com/ModOrganizer2/modorganizer/releases/download/v2.2.2/Mod.Organizer-2.2.2.0.7z";
            
        public async Task InstallManager(string installDir)
        {
            throw new System.NotImplementedException();
        }

        public async Task InstallMod(IMod mod)
        {
            
        }
    }
}