using System.Collections.Generic;
using System.Threading.Tasks;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install.Intefaces
{
    public interface IValidate : IModel
    {
        Task<List<ExtendedMod>> GetMissingModsAsync(params string[] directoriesToScan);
        List<ExtendedMod> GetMissingMods(params string[] directoriesToScan);
    }
}