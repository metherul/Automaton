using System.IO;
using System.Threading.Tasks;

namespace Gearbox.Shared.FsExtensions
{
    public static class DirectoryExt
    {
        public static Task<string[]> GetDirectoriesAsync(string directory, string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Task.Run(() => Directory.GetDirectories(directory, searchPattern, searchOption));
        }

        public static Task<string[]> GetFilesAsync(string directory, string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Task.Run(() => Directory.GetFiles(directory, searchPattern, searchOption));
        }

        public static async Task DeleteAsync(string directory)
        {
            var directoryContents = await GetFilesAsync(directory, "*", SearchOption.AllDirectories);

            foreach (var file in directoryContents)
            {
                using var fileStream = new FileStream(file, FileMode.Truncate, FileAccess.ReadWrite, FileShare.Delete, 1,
                                                      FileOptions.DeleteOnClose | FileOptions.Asynchronous);
                await fileStream.FlushAsync();
            }

            Directory.Delete(directory, true);
        }
    }
}
