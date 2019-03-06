using System.Threading.Tasks;
using Autofac;
using Automaton.Model.NexusApi.Interfaces;

namespace Automaton.Model.NexusApi
{
    public class ApiEndpoints
    {
        private readonly IApiBase _apiBase;

        public ApiEndpoints(IComponentContext components)
        {
            _apiBase = components.Resolve<IApiBase>();
        }

        public void GenerateModDownloadLink(string modId, string fileId)
        {

        }

        public void GenerateModDownloadLink(PipedData pipedData)
        {

        }

        private string MakeGenericeApiCall(string url)
        {


            return null;
        }
    }
}
