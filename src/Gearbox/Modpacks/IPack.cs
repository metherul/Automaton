using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gearbox.Modpacks
{
    public interface IPack
    {
        string PackType { get; }
        string Developer { get; }
        string Version { get; }

        IHeader Header { get; set; }
        ITheme Theme { get; set; }
        List<IMod> Mods { get; set; }
        List<ISource> Sources { get; set; }

        Task FromFile(string filePath);
    }
}