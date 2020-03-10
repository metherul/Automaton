using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gearbox.Archives
{
    public interface IArchive
    {
        public async Task Extract(string archiveFile, string extractDir)
        {
            return;
        }
    }
}
