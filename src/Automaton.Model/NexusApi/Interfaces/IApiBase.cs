using System.Threading.Tasks;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IApiBase : IService
    {
        event ApiBase.HasLoggedIn HasLoggedInEvent;

        string ApiKey { get; set; }
        int RemainingDailyRequests { get; set; }

        Task<bool> InitializeAsync(string apiKey = "");
        bool Initialize(string apiKey = "");
        bool IsUserLoggedIn();
        bool IsUserPremium();
    }
}