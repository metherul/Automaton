using System.Threading.Tasks;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IApiBase : IService
    {
        event ApiBase.HasLoggedIn HasLoggedInEvent;

        string ApiKey { get; set; }
        string GameName { get; set; }
        int RemainingDailyRequests { get; set; }

        Task<bool> InitializeAsync(string gameName, string apiKey = "");
        bool Initialize(string gameName, string apiKey = "");
    }
}