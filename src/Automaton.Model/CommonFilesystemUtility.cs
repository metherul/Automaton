using Automaton.Model.Interfaces;
using Alphaleonis.Win32.Filesystem;

namespace Automaton.Model
{
    public class CommonFilesystemUtility : ICommonFilesystemUtility
    {
        public void DeleteDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                File.SetAttributes(file, System.IO.FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(path, false);
        }
    }
}
