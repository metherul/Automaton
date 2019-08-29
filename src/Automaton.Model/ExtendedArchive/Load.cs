using Alphaleonis.Win32.Filesystem;
using Automaton.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public partial class ExtendedArchive
    {
        public void TryLoad(string archivePath)
        {
            // For performance we can check the file length first
            if (File.GetSize(archivePath) != Size)
            {
                return;
            }

            // We want to try to load in the archive, but we want to check it against the metadata we have stored first
            if (Utils.FileSHA256(archivePath) != SHA256)
            {
                return;
            }

            // We're good. This isn't quick, but it's accurate. We can add on further logic here further down the line.
            ArchivePath = archivePath;

            IsValidationComplete = true;
        }

        public async Task TryLoadAsync(string archivePath)
        {
            await Task.Run(() => TryLoad(archivePath));
        }

        public void SearchInDir(string directoryPath)
        {
            var dirContents = new Queue<string>(Directory.GetFiles(directoryPath, "*.*", System.IO.SearchOption.TopDirectoryOnly));

            while (!File.Exists(ArchivePath) && dirContents.Any())
            {
                var file = dirContents.Dequeue();

                TryLoad(file);
            }
        }

        public async Task SearchInDirAsync(string directoryPath)
        {
            await Task.Run(() => SearchInDir(directoryPath));
        }

        public void SearchInDirThreaded(string directoryPath)
        {
            var thread = new Thread(() => SearchInDir(directoryPath));
            thread.Start();
        }
    }
}
