using LanguageExt;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Gearbox.IO
{
    public class AsyncFs
    {
        public static async Task<ICollection<string>> GetDirectoryFiles(string dir, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return await Task.Run(() => Directory.GetFiles(dir, searchPattern, searchOption));
        }

        public static Task<string[]> GetDirectories(string dir, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Task.Run(() => Directory.GetDirectories(dir, searchPattern, searchOption));
        }

        public static async Task DeleteDirectory(string dir, bool recursive = false)
        {
            await Task.Run(() => Directory.Delete(dir, recursive));
        }

        public static Task DeleteFile(string path)
        {
            return Task.Run(() => File.Delete(path));
        }

        public static Task<string[]> GetDirectories(string dir)
        {
            return Task.Run(() => Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly));
        }

        public static async Task<string> GetFilesystemHash(string dir)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var hash = new StringBuilder();
            var files = await GetDirectoryFiles(dir, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                hash.Append(fileInfo.Name);
                hash.Append(fileInfo.Length);
            }

            var stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(hash.ToString()));
            var result = await FileHash.GetMd5Async(stream);

            Debug.WriteLine($"{dir} hashed in {stopwatch.ElapsedMilliseconds} ms.");
            return result;
        }
    }
}
                   