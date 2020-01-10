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
    internal class ArchiveHeader : IIndexHeader
    {
        public string Name { get; set; }
        public string RawPath { get; set; }
        
        [JsonConverter(typeof(ConcreteConverter<IndexEntry[]>))]
        public IIndexEntry[] IndexEntries { get; set; }

        [JsonConstructor]
        public ArchiveHeader()
        {
        }
        
        public ArchiveHeader(string archivePath)
        {
            Name = Path.GetFileName(archivePath);
            RawPath = archivePath;
        } 

        public async Task Build(string dir)
        {
            var files = await AsyncFs.GetDirectoryFiles(dir, "*", SearchOption.AllDirectories);
            IndexEntries = files.Select(x => EntryFactory.Create(PathExtensions.GetRelativePath(x, dir), x)).ToArray();

            var tasks = new List<Task>();

            // await Task.Run(() => Parallel.ForEach(IndexEntries, (entry) => { entry.Build().Wait(); }));
            
            foreach (var entry in IndexEntries)
            {
                //Debug.WriteLine($"Building entry: {entry.RelativeFilePath}");
            
                tasks.Add(entry.Build());
            }
            
            await Task.WhenAll(tasks.ToArray());
        }
    }
}