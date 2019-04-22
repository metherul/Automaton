using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Modpack;
using SharpCompress.Archives;

namespace Automaton.Model.Install
{
    public class InstallBase : IInstallBase
    {
        public BitmapImage HeaderImage { get; set; }

        public string InstallDirectory { get; set; }
        public string DownloadsDirectory { get; set; }

        public List<IArchiveEntry> ModpackContents { get; set; }

        public ModPackMasterDefinition ModPackMasterDefinition { get; set; }
        public List<ExtendedCompiledMod> ModpackMods { get; set; } = new List<ExtendedCompiledMod>();
    }
}
