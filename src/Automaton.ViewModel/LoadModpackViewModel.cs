using Autofac;
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
        private readonly IDialogController _dialogController;

        public RelayCommand ChooseModpackCommand { get => new RelayCommand(ChooseModpack); }

        public LoadModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _filesystemBrowser = components.Resolve<IFileSystemBrowser>(); 
            _dialogController = components.Resolve<IDialogController>();
        }

        private async void ChooseModpack()
        {
            var modpackPath = await _filesystemBrowser.OpenFileBrowserAsync("Automaton Modpacks (*.auto, *.7z, *.rar, *.zip) | *.auto; *.7z; *.rar; *.zip|All Files (*.*)|*.*", "Select an Automaton Modpack");

            if (string.IsNullOrEmpty(modpackPath))
            {
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
