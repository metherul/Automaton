using System.Threading.Tasks;

namespace Automaton.Model.Interfaces
{
    public interface INexusApi : ISingleton
    {
        Task<string> GetArchiveDownloadUrl(ExtendedArchive archive, string protocolParams = "");
        Task Init(string key);
        Task<bool> IsPremiumUser();
    }
}