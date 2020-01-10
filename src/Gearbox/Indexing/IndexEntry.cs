using Gearbox.Indexing.Interfaces;
using Gearbox.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Gearbox.Indexing
{
    public class IndexEntry : IIndexEntry
    {
        private string _filePath;

        public string RelativeFilePath { get; set; }
        public string Hash { get; set; }
        public DateTime LastModified { get; set; }
        public long Length { get; set; }

        public IndexEntry(string relativeFilePath, string filePath)
        {
            _filePath = filePath;
            RelativeFilePath = relativeFilePath;
        }

        public async Task Build()
        {
            var fileInfo = new FileInfo(_filePath);

            Debug.WriteLine("Building: " + _filePath); 

            Hash = await Task.Run(() => FileHash.GetMd5(File.OpenRead(_filePath)));
            LastModified = fileInfo.LastWriteTime;
            Length = fileInfo.Length;
        }
    }
}
