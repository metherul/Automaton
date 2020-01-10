using System.IO;
using System.Threading.Tasks;
using Gearbox.IO;
using Gearbox.Modpacks.OMS.Base;
using Gearbox.Repositories;
using Newtonsoft.Json.Linq;

namespace Gearbox.Modpacks.OMS
{
    public class Source : ISource, IJsonSource
    {
        public string Name => _sourceBase?.Name;
        public string Author => throw new System.NotImplementedException();
        public string FilePath => _downloadPath;

        private IRepository _repository;
        public IRepository Repository
        {
            get
            {
                if (_repository != null)
                {
                    return _repository;
                }

                _repository = RepositoryFactory.FromJson(_sourceBase.Repository as JObject);

                return _repository;
            }
        }

        private SourceBase _sourceBase;
        private string _downloadPath;
        
        public async Task FromJson(Stream stream)
        {
            _sourceBase = await JsonUtils.ReadJson<SourceBase>(stream);
        }
        
        public async Task DownloadFile(string downloadPath)
        {
            _downloadPath = downloadPath;
        }

        public async Task Validate()
        {
            throw new System.NotImplementedException();
        }

        public async Task Register()
        {
            throw new System.NotImplementedException();
        }

    }
}