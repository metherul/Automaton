using System.Collections.Generic;

namespace Gearbox.Modpacks
{
    public interface IMod
    {
        string Name { get; }
        string[] Sources { get; }
        IReadOnlyList<InstallEntry> InstallEntries { get; }
    }
}