using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Automaton.ViewModel.Controllers.Interfaces;

namespace Automaton.ViewModel.Controllers
{
    public class ThemeController : IThemeController
    {
        public void ApplyTheme()
        {
            var modpackHeader = Model.Instance.AutomatonInstance.ModpackHeader;

            if (!string.IsNullOrEmpty(modpackHeader.BackgroundColor))
            {
                Application.Current.Resources["BackgroundColor"] =
                    (SolidColorBrush)new BrushConverter().ConvertFromString(modpackHeader.BackgroundColor);
            }

            if (!string.IsNullOrEmpty(modpackHeader.FontColor))
            {
                Application.Current.Resources["FontColor"] = (SolidColorBrush)new BrushConverter().ConvertFromString(modpackHeader.FontColor);
            }

            if (!string.IsNullOrEmpty(modpackHeader.ButtonColor))
            {
                Application.Current.Resources["ButtonColor"] = (SolidColorBrush)new BrushConverter().ConvertFromString(modpackHeader.ButtonColor);
            }

            if (!string.IsNullOrEmpty(modpackHeader.ControlColor))
            {
                Application.Current.Resources["AssistantControlColor"] = (SolidColorBrush)new BrushConverter().ConvertFromString(modpackHeader.ControlColor);
            }

            if (!string.IsNullOrEmpty(modpackHeader.HeaderImage))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                bitmapImage.UriSource = new Uri(modpackHeader.HeaderImage);

                bitmapImage.EndInit();

                Application.Current.Resources["HeaderImage"] = bitmapImage;
            }

            Application.Current.Resources["ModpackName"] = modpackHeader.ModpackName;
            Application.Current.Resources["ModpackDescription"] = modpackHeader.Description;
        }
    }
}