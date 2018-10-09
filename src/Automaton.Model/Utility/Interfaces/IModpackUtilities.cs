using System;
using Automaton.Model.Interfaces;
using Automaton.Model.ModpackBase;

namespace Automaton.Model.Utility.Interfaces
{
    public interface IModpackUtilties : IModel
    {
        void InstallModpack(IProgress<InstallModpackProgress> progress);
        bool LoadModpack(string modpackPath);
        void UpdateModArchivePaths(Mod mod, string archivePath);
    }
}