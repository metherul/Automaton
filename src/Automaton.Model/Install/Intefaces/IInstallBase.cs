using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install.Intefaces
{
    public interface IInstallBase : IService
    {
        BitmapImage HeaderImage { get; set; }

        string DownloadsDirectory { get; set; }
        string InstallDirectory { get; set; }
        string PluginsTxt { get; set; }
        string LoadorderTxt { get; set; }
        string ModlistTxt { get; set; }
        string ArchivesTxt { get; set; }
        Header ModpackHeader { get; set; }
        List<ExtendedMod> ModpackMods { get; set; }
    }
}