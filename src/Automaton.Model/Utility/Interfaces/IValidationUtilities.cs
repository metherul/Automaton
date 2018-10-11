using System.Collections.Generic;
using System.Threading.Tasks;
using Automaton.Model.Interfaces;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.Utility.Interfaces
{
    public interface IValidationUtilities : IModel
    {
        List<IMod> MissingMods { get; set; }

        event ValidationUtilities.ValidateSourcesUpdate ValidateSourcesUpdateEvent;

        List<IMod> GetMissingMods(List<string> sourceFiles);
        Task<List<IMod>> GetMissingModsAsync(List<string> sourceFiles);
        List<string> GetSourceFiles();
        Task<bool> IsMatchingModArchive(IMod mod, string archivePath);
        Task<bool> IsMatchingModArchiveAsync(IMod mod, string archivePath);
    }
}