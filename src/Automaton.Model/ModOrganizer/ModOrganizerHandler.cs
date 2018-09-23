using Automaton.Model.Instance;
using System.IO;
using System.Threading.Tasks;

namespace Automaton.Model.ModOrganizer
{
    public class ModOrganizerHandler
    {
        public ModOrganizerHandler()
        {
            
        }

        public Task InstallModOrganizer()
        {
            if (!AutomatonInstance.ModpackHeader.InstallModOrganizer)
            {
                return null;
            }

            // Extract MO into the target folder


            // Set the InstallLocation in the instance to the mod subdirectory. 
            AutomatonInstance.InstallLocation = Path.Combine(AutomatonInstance.InstallLocation, "mods");

            return null;
        }
    }
}
