using System.IO;
using System.Threading.Tasks;
using Automaton.Model.Instance.Interfaces;

namespace Automaton.Model.ModOrganizer
{
    public class ModOrganizerHandler
    {
        private readonly IAutomatonInstance _automatonInstance;

        public ModOrganizerHandler(IAutomatonInstance automatonInstance)
        {
            _automatonInstance = automatonInstance;
        }

        public Task InstallModOrganizer()
        {
            if (!_automatonInstance.ModpackHeader.InstallModOrganizer)
            {
                return null;
            }

            // Extract MO into the target folder


            // Set the InstallLocation in the instance to the mod subdirectory. 
            _automatonInstance.InstallLocation = Path.Combine(_automatonInstance.InstallLocation, "mods");

            return null;
        }
    }
}
