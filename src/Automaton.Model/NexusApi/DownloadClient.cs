using System.Threading.Tasks;
using Automaton.Model.Install;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.NexusApi.Interfaces;

namespace Automaton.Model.NexusApi
{
    public class DownloadClient : IDownloadClient
    {
        private readonly IInstallBase _installBase;

        public async Task<bool> DownloadFileAsync(string downloadUrl, ExtendedMod callback)
        {
            return false;
        }
    }
}
