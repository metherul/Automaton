using System.Collections.Generic;
using System.Threading.Tasks;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install.Intefaces
{
    public interface IValidate : IModel
    {
        Task<List<ExtendedMod>> GetMissingModsAsync(List<string> directoriesToScan);
        List<ExtendedMod> GetMissingMods(List<string> directoriesToScan);
        List<ExtendedMod> FilterMissingMods(string directoryPath);
        Task<List<ExtendedMod>> FilterMissingModsAsync(string directoryPath);
        List<ExtendedMod> ValidateTargetModArchive(string archivePath, List<ExtendedMod> missingMods);
    }
}