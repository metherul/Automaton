using Automaton.Model.Interfaces;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.Utility.Interfaces
{
    public interface IJsonUtilities : IModel
    {
        IHeader DeserializeHeader(string headerContents, out string parseError);
        IMod DeserializeMod(string modContents, out string parseError);
    }
}