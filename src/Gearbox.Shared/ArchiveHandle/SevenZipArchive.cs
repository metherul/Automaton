using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Gearbox.Shared.ArchiveHandle
{
    public class SevenZipArchive : IArchive
    {
        public Task Extract(string extractDir)
        {
            throw new NotImplementedException();
        }

        public Task Extract(string extractDir, ExtractOptions extractOptions)
        {
            throw new NotImplementedException();
        }
    }
}
