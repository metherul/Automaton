using Autofac;
using Automaton.Model.Archive.Interfaces;
using Automaton.Model.Modpack.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automaton.Model.Modpack.Base;
using Automaton.Model.Modpack.Base.Interfaces;

namespace Automaton.Model.Modpack
{
    public class ModpackRead : IModpackRead
    {
        private readonly IArchiveContents _archiveContents;
        private readonly IModpackValidate _modpackValidate;
        private readonly IModpackStructure _modpackStructure;

        public ModpackRead(IComponentContext components)
        {
            _archiveContents = components.Resolve<IArchiveContents>(); ;
            _modpackValidate = components.Resolve<IModpackValidate>(); ;
            _modpackStructure = components.Resolve<IModpackStructure>(); ;
        }

        public async Task<bool> LoadModpackAsync(string modpackPath)
        {
            return await Task.Run(() => LoadModpack(modpackPath));
        }

        public bool LoadModpack(string modpackPath)
        {
            var modpackEntries = _archiveContents.GetArchiveEntries(modpackPath);
            var (isValidateSuccessful, validateErrorMessage) = _modpackValidate.ValidateCorrectModpackStructure(modpackEntries);

            if (!isValidateSuccessful)
            {
                return false;
            }

            var header = modpackEntries.First(x => x.FileName == _modpackStructure.HeaderOffset);

            if (header is null)
            {
                return false;
            }

            var headerMemoryStream = new MemoryStream();

            header.Extract(headerMemoryStream);
            var headerObject = ConsumeModpackJsonFile<Header>(headerMemoryStream);

            return true;
        }

        private T ConsumeModpackJsonFile<T>(MemoryStream memoryStream) where T : class
        {
            var jsonString = Encoding.ASCII.GetString(memoryStream.ToArray());
            var jsonObject = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }) as T;

            return jsonObject;
        }

        private bool InjectBitmapImages()
        {
            return (false, "");
        }
    }
}