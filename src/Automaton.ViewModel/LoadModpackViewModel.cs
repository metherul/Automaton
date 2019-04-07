using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Automaton.ViewModel
{
    public class LoadModpackViewModel : ILoadModpackViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _filesystemBrowser;
        private readonly IModpackRead _modpackRead;
        private readonly IDialogController _dialogController;
        private readonly ILogger _logger;
        private readonly IInstallBase _installBase;
        private readonly IPathFix _pathFix;

        public RelayCommand ChooseModpackCommand { get => new RelayCommand(ChooseModpack); }

        public LoadModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _filesystemBrowser = components.Resolve<IFileSystemBrowser>(); 
            _modpackRead = components.Resolve<IModpackRead>(); 
            _dialogController = components.Resolve<IDialogController>();
            _logger = components.Resolve<ILogger>();
            _installBase = components.Resolve<IInstallBase>();

            _pathFix = components.Resolve<IPathFix>();
        }

        private async void ChooseModpack()
        {
            _logger.WriteLine("Opening modpack filesystemBrowser");

            var modpackPath = await _filesystemBrowser.OpenFileBrowserAsync("Automaton Modpacks (*.auto, *.7z, *.rar, *.zip) | *.auto; *.7z; *.rar; *.zip|All Files (*.*)|*.*", "Select an Automaton Modpack");

            if (string.IsNullOrEmpty(modpackPath))
            {
                _logger.WriteLine("modpackPath is null or empty");

                return;
            }

            var isSuccessful = await _modpackRead.LoadModpackAsync(modpackPath);

            if (!isSuccessful)
            {
                _logger.WriteLine("Modpack load was not successful");

                return;
            }

            // Apply theme
            ApplyTheme();

            _viewController.IncrementCurrentViewIndex();
        }

        private void ApplyTheme()
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries; //{Resources/Themes/DarkTheme.xaml}
            var resourceIndex = mergedDictionaries.IndexOf(mergedDictionaries.First(x => x.Source.ToString() == "Resources/Themes/DarkTheme.xaml"));
            var themeDictionary = mergedDictionaries[resourceIndex];

            // Apply themes
            var modpackHeader = _installBase.ModpackHeader;
            if (modpackHeader.BackgroundColor != null)
            {
                themeDictionary["BackgroundColor"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(modpackHeader.BackgroundColor));
            }

            if (modpackHeader.FontColor != null)
            {
                themeDictionary["TextColor"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(modpackHeader.FontColor ?? themeDictionary["TextColor"]));
            }

            if (modpackHeader.ButtonColor != null)
            {
                themeDictionary["ButtonColor"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(modpackHeader.ButtonColor ?? themeDictionary["ButtonColor"]));
            }

            // Apply textual elements
            var resourceDictionary = Application.Current.Resources;
            resourceDictionary["ModpackName"] = modpackHeader.Name ?? resourceDictionary["ModpackName"];
            resourceDictionary["ModpackDescription"] = modpackHeader.Description ?? resourceDictionary["ModpackDescription"];

            // Apply header image
            resourceIndex = mergedDictionaries.IndexOf(mergedDictionaries.First(x => x.Source.ToString() == "Resources/Images/ImageResources.xaml"));
            themeDictionary = mergedDictionaries[resourceIndex];

            themeDictionary["HeaderImage"] = _installBase.HeaderImage ?? themeDictionary["HeaderImage"];

            resourceDictionary["AutomatonVersion"] = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }
    }
}
