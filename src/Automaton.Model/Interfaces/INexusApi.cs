using System.Threading.Tasks;

namespace Automaton.Model.Interfaces
{
    public interface INexusApi : ISingleton
    {
        bool IsPremium { get; set; }
        bool IsLoggedIn { get; set; }

        Task<string> GetArchiveDownloadUrl(ExtendedArchive archive, string protocolParams = "");
        Task Init(string key);
        Task<bool> IsPremiumUser();
    }
}