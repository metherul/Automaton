using Gearbox.Indexing.Next.Model;
using Gearbox.IO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gearbox.Indexing.Next
{
    public class IndexWriter
    {
        private Index _index;

        public IndexWriter(Index index) => _ = _index;

        public async Task GenerateNew()
        {
            // Delete our pre-existing index.
            if (File.Exists(_index.IndexPath))
            {
                await AsyncFs.DeleteFile(_index.IndexPath);
            }

            var indexRoot = new IndexRoot();
            indexRoot.Mods = (await AsyncFs.GetDirectories(_index.ModsDir))
                .Select(x => new IndexMod()
                {
                    Name = new DirectoryInfo(x).Name,
                    Path = x,
                })
                .ToList();

            foreach (var mod in indexRoot.Mods)
            {
                var modFiles = await AsyncFs.GetDirectoryFiles(mod.Path, "*", SearchOption.AllDirectories);

                foreach (var file in modFiles)
                {
                    var fileInfo = new FileInfo(file);
                    var entry = new IndexFile()
                    {
                        CRC = (await FileHash.GetCrcAsync(File.OpenRead(file))).ToString(),
                        SHA = await FileHash.GetMd5Async(File.OpenRead(file)),
                        LastModified = fileInfo.LastWriteTime,
                        Name = fileInfo.Name,
                        Path = fileInfo.FullName,
                        Length = fileInfo.Length
                    };

                    mod.Contents.Add(entry);
                }

            }

            // Dump the results to the index file.
            var index = Path.Combine(_index.ManagerDir, $"index-{DateTime.UtcNow}.automaton");
            await JsonUtils.WriteJson(indexRoot, index);
        }
    }
}
