using System.IO;
using System.Reflection;

namespace Automaton.Model.Utility
{
    public class Resources
    {
        public static void WriteResourceToFile(string resourceName, string fileName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }

            using (var resource = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }
    }
}
