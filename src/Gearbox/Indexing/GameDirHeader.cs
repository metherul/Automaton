using Gearbox.Indexing.Interfaces;
using Gearbox.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gearbox.Indexing
{
    public class GameDirHeader : IIndexHeader
    {
        public string Name { get; set; }
        public string RawPath { get; set; }
        
        [JsonConverter(typeof(ConcreteConverter<IndexEntry[]>))]
        public IIndexEntry[] IndexEntries { get; set; }
        
        [JsonConstructor]
        public GameDirHeader()
        {
        }
        public GameDirHeader(string path)
        {
            Name = new DirectoryInfo(path).Name;
            RawPath = path;
        }

        public async Task Build(string dir)
        {
            var files = await AsyncFs.GetDirectoryFiles(dir, "*", SearchOption.AllDirectories);
            IndexEntries = files.Select(x => EntryFactory.Create(PathExtensions.GetRelativePath(x, dir), x)).ToArray();

            var tasks = new List<Task>();

            foreach (var entry in IndexEntries)
            {
                Debug.WriteLine(entry.RelativeFilePath);

                tasks.Add(entry.Build());
            }

            Debug.WriteLine("Waiting for all tasks to finish...");
            await Task.WhenAll(tasks.ToArray());
        }
    }
}
