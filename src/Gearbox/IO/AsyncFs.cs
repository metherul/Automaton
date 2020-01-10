using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gearbox.IO
{
    public class AsyncFs
    {
        public static async Task<ICollection<string>> GetDirectoryFiles(string dir, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return await Task.Run(() => Directory.GetFiles(dir, searchPattern, searchOption));
        }

        public static async Task<ICollection<string>> GetDirectories(string dir, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return await Task.Run(() => Directory.GetDirectories(dir, searchPattern, searchOption));
        }

        public static async Task DeleteDirectory(string dir, bool recursive = false)
        {
            await Task.Run(() => Directory.Delete(dir, recursive));
        }
    }
}
                   