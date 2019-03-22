using System;
using System.Diagnostics;
using Autofac;
using Automaton.Model.Archive.Interfaces;
using Automaton.Model.Modpack.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Automaton.Model.Install;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base;
using Automaton.Model.Modpack.Base.Interfaces;

namespace Automaton.Model.Modpack
{
    public class ModpackRead : IModpackRead
    {
        private readonly IArchiveContents _archiveContents;
        private readonly IModpackValidate _modpackValidate;
        private readonly IModpackStructure _modpackStructure;
        private readonly IInstallBase _installBase;
        private readonly IClassExtensions _classExtensions;

        public ModpackRead(IComponentContext components)
        {
            _archiveContents = components.Resolve<IArchiveContents>(); 
            _modpackValidate = components.Resolve<IModpackValidate>(); 
            _modpackStructure = components.Resolve<IModpackStructure>(); 
            _installBase = components.Resolve<IInstallBase>();
            _classExtensions = components.Resolve<IClassExtensions>();
        }

        public async Task<bool> LoadModpackAsync(string modpackPath)
        {
            return await Task.Factory.StartNew(() => LoadModpack(modpackPath));
        }

        public bool LoadModpack(string modpackPath)
        {
            var modpackEntries = _archiveContents.GetArchiveEntries(modpackPath);
            var isValidateSuccessful = _modpackValidate.ValidateCorrectModpackStructure(modpackEntries);

            if (!isValidateSuccessful)
            {
                return false;
            }

            var header = modpackEntries.First(x => x.Key == _modpackStructure.HeaderOffset);

            if (header is null)
            {
                return false;
            }

            // Load modpack header
            var headerMemoryStream = new MemoryStream();
            header.OpenEntryStream().CopyTo(headerMemoryStream);

            var headerObject = ConsumeModpackJsonFile<Header>(headerMemoryStream);

            //if (headerObject.Version != "1.0b")
            //{
            //    return false;
            //}

            _installBase.ModpackHeader = headerObject;

            // Load each modpack config file
            var modDirectory = _installBase.ModpackHeader.ModInstallFolders.First();
            var modFileEntries = modpackEntries.Where(x => x.Key.StartsWith(modDirectory) && !x.IsDirectory).ToList();

            foreach (var modFile in modFileEntries)
            {
                var modFileMemoryStream = new MemoryStream();
                modFile.OpenEntryStream().CopyTo(modFileMemoryStream);

                var mod = ConsumeModpackJsonFile<Mod>(modFileMemoryStream);

                _installBase.ModpackMods.Add(_classExtensions.ToDerived<Mod, ExtendedMod>(mod));
            }

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
            return false;
        }
    }
}