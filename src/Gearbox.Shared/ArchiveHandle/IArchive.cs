using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gearbox.Shared.ArchiveHandle
{
    public interface IArchive
    {
        Task Extract(string extractDir);

        Task Extract(string extractDir, ExtractOptions extractOptions);
    }
}
