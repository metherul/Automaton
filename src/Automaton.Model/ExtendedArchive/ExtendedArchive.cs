using Autofac;
using Automaton.Model.Interfaces;
using SevenZipExtractor;
using System.Collections.Generic;
using System.Net;
using Automaton.Common.Model;
using System.ComponentModel;
using Automaton.Model.HandyUtils.Interfaces;

namespace Automaton.Model
{
    public partial class ExtendedArchive : SourceArchive, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Mod _parentMod;

        private WebClient _webClient;

        private IArchiveHandle _archiveHandle;
        private ILifetimeData _lifetimeData;
        private INexusApi _nexusApi;
        private IDialogRedirector _dialogRedirector;

        private ExtendedArchive _boundArchive;

        public string ArchivePath { get; set; }
        public string DisplayName { get; set; }
        public bool IsValidationComplete { get; set; }

        public InstallerType InstallerType { get; set; }

        public Dictionary<string, Entry> Patches { get; private set; }

        public ExtendedArchive Initialize(IComponentContext components, Mod parentMod, Dictionary<string, SevenZipExtractor.Entry> patches,
                                            InstallerType installerType = InstallerType.Mod)
        {
            // Load in required modules
            _parentMod = parentMod;

            _archiveHandle = components.Resolve<IArchiveHandle>();
            _lifetimeData = components.Resolve<ILifetimeData>();
            _nexusApi = components.Resolve<INexusApi>();
            _dialogRedirector = components.Resolve<IDialogRedirector>();
            Patches = patches;

            DisplayName = Name;

            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = ArchiveName;
            }

            return this;
        }

        public void BindToOther(ExtendedArchive archive)
        {
            // So in some instances we may have multiple ExtendedArchives that all source back to a single archive file. We limit this to one, 
            // and just save the class's ref

            _boundArchive = archive;
            IsValidationComplete = true; // We can get this out of the way, since we cannot continue the installation until the bound archive is validated
        }
    }
}
