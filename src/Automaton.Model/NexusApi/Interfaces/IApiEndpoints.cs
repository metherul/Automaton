using System.Threading.Tasks;
using Automaton.Model.Install;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IApiEndpoints : IService
    {
        Task<string> GenerateModDownloadLinkAsync(PipedData pipedData);
        Task<string> GenerateModDownloadLinkAsync(ExtendedMod mod);
        Task<string> GenerateModDownloadLinkAsync(string gameName, string modId, string fileId);
    }
}