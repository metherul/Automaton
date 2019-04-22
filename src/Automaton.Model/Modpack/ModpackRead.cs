using Autofac;
using Automaton.Model.Archive.Interfaces;
using Automaton.Model.Modpack.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Interfaces;
using System.Reflection;
using System.Collections.Generic;
using SharpCompress.Archives;
using System.Windows.Media.Imaging;

namespace Automaton.Model.Modpack
{
    public class ModpackRead : IModpackRead
    {
        private readonly IComponentContext _components;

        private readonly IArchiveContents _archiveContents;
        private readonly IModpackValidate _modpackValidate;
        private readonly IInstallBase _installBase;
        private readonly IClassExtensions _classExtensions;
        private readonly ILogger _logger;

        public ModpackRead(IComponentContext components)
        {
            _components = components;

            _archiveContents = components.Resolve<IArchiveContents>(); 
            _modpackValidate = components.Resolve<IModpackValidate>(); 
            _installBase = components.Resolve<IInstallBase>();
            _classExtensions = components.Resolve<IClassExtensions>();
            _logger = components.Resolve<ILogger>();
        }

        public async Task<bool> LoadModpackAsync(string modpackPath)
        {
            return await Task.Factory.StartNew(() => LoadModpack(modpackPath));
        }

        public bool LoadModpack(string modpackPath)
        {
            var archive = _archiveContents.GetArchive(modpackPath);
            var modpackEntries = _archiveContents.GetArchiveEntries(archive);
            var isValidateSuccessful = _modpackValidate.ValidateCorrectModpackStructure(modpackEntries);

            if (!isValidateSuccessful)
            {
                _logger.WriteLine("Validate failure");

                return false;
            }

            _installBase.ModpackContents = modpackEntries;

            var masterDefinition = modpackEntries.First(x => x.Key == ConfigPathOffsets.PackDefinitionConfig);
            if (masterDefinition == null)
            {
                _logger.WriteLine("Header is null");

                return false;
            }

            // Load modpack header
            var masterDefinitionStream = new MemoryStream();
            masterDefinition.OpenEntryStream().CopyTo(masterDefinitionStream);

            var masterDefinitionObject = ConsumeModpackJsonFile<ModPackMasterDefinition>(masterDefinitionStream);

            if (masterDefinitionObject.StandardTarget != "2.0b")
            {
                _logger.WriteLine("ModpackVersion mismatch");

                return false;
            }

            _installBase.ModPackMasterDefinition = masterDefinitionObject;

            // Load each modpack config file
            var modFileEntries = modpackEntries.Where(x => x.Key.StartsWith(modDirectory) && !x.IsDirectory).ToList();

            foreach (var modFile in modFileEntries)
            {
                var modFileMemoryStream = new MemoryStream();
                modFile.OpenEntryStream().CopyTo(modFileMemoryStream);

                var mod = ConsumeModpackJsonFile<Mod>(modFileMemoryStream);
                var extendedMod = _classExtensions.ToDerived<Mod, ExtendedMod>(mod);

                extendedMod.DisplayName = extendedMod.NexusFileName;

                if (string.IsNullOrEmpty(extendedMod.NexusFileName))
                {
                    extendedMod.DisplayName = extendedMod.ModName;
                }

                if (string.IsNullOrEmpty(extendedMod.Version))
                {
                    extendedMod.Version = "?";
                }

                extendedMod.Initialize(_components);

                _installBase.ModpackMods.Add(extendedMod);
            }

            var test = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            // Load an extended mod object for the specified mod organizer item
            var assembly = Assembly.GetExecutingAssembly();
            var fileName = assembly.GetName().Name + ".ModOrganizer.ModOrganizer" + headerObject.ModOrganizerVersion.ToString() + ".json";

            using (var fileStream = assembly.GetManifestResourceStream(fileName))
            {
                var memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);

                memoryStream.Position = 0;

                var modOrganizerObject = ConsumeModpackJsonFile<Mod>(memoryStream);
                var extendedModOrganizerObject = _classExtensions.ToDerived<Mod, ExtendedMod>(modOrganizerObject);

                extendedModOrganizerObject.IsModOrganizer = true;
                extendedModOrganizerObject.DisplayName = extendedModOrganizerObject.NexusFileName;
                extendedModOrganizerObject.Initialize(_components);

                _installBase.ModpackMods.Add(extendedModOrganizerObject);
            }

            foreach (var entry in modpackEntries.Where(x => !x.IsDirectory))
            {
                if (!entry.IsDirectory && entry.Key.StartsWith("Resources/header."))
                {
                    var memoryStream = new MemoryStream();
                    entry.WriteTo(memoryStream);

                    var bitmapImage = InjectBitmapImages(memoryStream);

                    _installBase.HeaderImage = bitmapImage;
                }
            }
            
            _logger.WriteLine("Modpack loaded");

            return true;
        }

        private T ConsumeModpackJsonFile<T>(MemoryStream memoryStream) where T : class
        {
            var jsonString = Encoding.ASCII.GetString(memoryStream.ToArray());

            if (jsonString.StartsWith("???"))
            {
                jsonString = jsonString.Remove(0, 3);
            }

            var jsonObject = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }) as T;

            return jsonObject;
        }

        private void LoadLoadOrderFiles(List<IArchiveEntry> modpackEntries)
        {
            var pluginsTxt = modpackEntries.First(x => x.Key == _modpackStructure.PluginsTxtOffset);
            var streamReader = new StreamReader(pluginsTxt.OpenEntryStream());

            _installBase.PluginsTxt = streamReader.ReadToEnd();

            var loadorderTxt = modpackEntries.First(x => x.Key == _modpackStructure.LoadorderTxtOffset);
            streamReader = new StreamReader(loadorderTxt.OpenEntryStream());

            _installBase.LoadorderTxt = streamReader.ReadToEnd();

            var modlistTxt = modpackEntries.First(x => x.Key == _modpackStructure.ModlistTxtOffset);
            streamReader = new StreamReader(modlistTxt.OpenEntryStream());

            _installBase.ModlistTxt = streamReader.ReadToEnd();

            var archivesTxt = modpackEntries.First(x => x.Key == _modpackStructure.ArchivesTxtOffset);
            streamReader = new StreamReader(archivesTxt.OpenEntryStream());

            _installBase.ArchivesTxt = streamReader.ReadToEnd();
        }

        private BitmapImage InjectBitmapImages(MemoryStream imageStream)
        {
            var bitmap = new BitmapImage();
            imageStream.Seek(0, SeekOrigin.Begin);

            bitmap.BeginInit();
            bitmap.StreamSource = imageStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
    }
}