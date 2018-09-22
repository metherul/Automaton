using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Automaton.Model
{
    public class ProcessFinder
    {
        public static bool IsProcessAlreadyRunning()
        {
            return Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1;
        }
    }
}
