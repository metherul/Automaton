using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.IO;
using Gearbox.Modpacks.OMS.Base;
using Gearbox.Repositories;
using LanguageExt;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;

using static LanguageExt.Prelude;

namespace Gearbox.Modpacks.OMS
{
    public class Source : ISource, IJsonSource
    {
        public string Name => _sourceBase?.Name;
        public string Author => throw new System.NotImplementedException();
        public string FilePath => _downloadPath;

        private Option<IRepository> _repository;
        public Option<IRepository> Repository
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
            await Repository.IfSomeAsync(async (f) =>
            {
                await f.DownloadFile(downloadPath);
            });
        }

        public async Task Validate()
        {
            throw new System.NotImplementedException();
        }

        public void Register(string archivePath)
        {
            _downloadPath = archivePath;
        }

        public async Task<Option<string>> FindMatchInDir(string dir)
        {
            var fileContents = Directory.GetFiles(dir);
            var fileLengthQuery = fileContents.Where(x => new FileInfo(x).Length == _sourceBase.Length);

            if (fileLengthQuery.Count() >= 1)
            {
                return fileLengthQuery.First();
            }

            return None;
        }

    }
}