using Gearbox.Indexing.Interfaces;
using System.IO;

namespace Gearbox.Indexing.Factories
{
    public class HeaderFactory
    {
        public static IIndexHeader Create(string path, bool isGameDir = false, bool isUtilitiesDir = false)
        {
            if (isGameDir)
            {
                return new GameDirHeader(path);
            }

            if (isUtilitiesDir)
            {
                return new UtilitiesHeader(path);
            }

            return File.Exists(path) switch 
            {
                true => new ArchiveHeader(path),
                false => new ModHeader(path)
            };
        }   
    }
}
