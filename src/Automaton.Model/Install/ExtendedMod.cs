using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install
{
    public class ExtendedMod : Mod
    {
        public string FilePath { get; set; }

        public int CurrentDownloadProgress { get; set; }

        public bool IsIndeterminateProcess { get; set; } = true;
    }
}
