using System.Threading.Tasks;

namespace Automaton.Model.Interfaces
{
    public interface INexusApi : ISingleton
    {
        void Init(string key);
        Task<bool> IsPremiumUser();
    }
}