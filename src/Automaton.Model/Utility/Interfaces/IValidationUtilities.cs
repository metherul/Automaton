using System.Collections.Generic;
using System.Threading.Tasks;
using Automaton.Model.Interfaces;
using Automaton.Model.ModpackBase;

namespace Automaton.Model.Utility.Interfaces
{
    public interface IValidationUtilities : IModel
    {
        List<Mod> MissingMods { get; set; }

        event ValidationUtilities.ValidateSourcesUpdate ValidateSourcesUpdateEvent;

        List<Mod> GetMissingMods(List<string> sourceFiles);
        Task<List<Mod>> GetMissingModsAsync(List<string> sourceFiles);
        List<string> GetSourceFiles();
        Task<bool> IsMatchingModArchive(Mod mod, string archivePath);
        Task<bool> IsMatchingModArchiveAsync(Mod mod, string archivePath);
    }
}