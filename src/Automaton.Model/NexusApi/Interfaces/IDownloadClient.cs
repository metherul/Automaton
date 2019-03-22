using System.Threading.Tasks;
using Automaton.Model.Install;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IDownloadClient : IService
    {
        Task<bool> DownloadFileAsync(string downloadUrl, ExtendedMod callback);
    }
}