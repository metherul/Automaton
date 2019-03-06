using System.Threading.Tasks;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IApiEndpoints
    {
        Task<string> GenerateModDownloadLinkAsync(PipedData pipedData);
        Task<string> GenerateModDownloadLinkAsync(string gameName, string modId, string fileId);
    }
}