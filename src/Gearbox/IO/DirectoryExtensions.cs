using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Gearbox.IO
{
    public class DirectoryExtensions
    {
        public static async Task ClearAttributes(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }

            await Task.Run(() =>
            {
                File.SetAttributes(dir, FileAttributes.Normal);

                var subDirs = Directory.EnumerateDirectories(dir);
                foreach (string subDir in subDirs)
                {
                    ClearAttributes(subDir);
                }

                var files = Directory.EnumerateFiles(dir);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
            });
        }
    }
}
