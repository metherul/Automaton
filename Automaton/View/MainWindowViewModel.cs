using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Automaton.Controllers;
using Automaton.Model.Instances;
using GalaSoft.MvvmLight.Command;

namespace Automaton.View
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand<Window> CloseWindowCommand { get; set; }
        public RelayCommand<Window> MinimizeWindowCommand { get; set; }
        public RelayCommand<Window> MoveWindowCommand { get; set; }

        public int CurrentTransitionerIndex { get; set; } = 0;

        public MainWindowViewModel()
        {
            // Initialize relaycommand bindings
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
            MinimizeWindowCommand = new RelayCommand<Window>(MinimizeWindow);
            MoveWindowCommand = new RelayCommand<Window>(MoveWindow);

            // Initialize event handlers
            ModpackInstance.ModpackHeaderChangedEvent += ModpackHeaderInstanceUpdate;
            ViewIndexController.ViewIndexChangedEvent += ViewIndexUpdate;
        }

        public void ModpackHeaderInstanceUpdate()
        {
            // Modpack has been loaded, so increment the current view index
            ViewIndexController.IncrementCurrentViewIndex();

            ApplyAutomatonTheme();
        }

        private void ViewIndexUpdate(int index)
        {
            CurrentTransitionerIndex = index;
        }

        private void ApplyAutomatonTheme()
        {
            var modpackHeader = ModpackInstance.ModpackHeader;

            if (!string.IsNullOrEmpty(modpackHeader.BackgroundColor))
            {
                Application.Current.Resources["BackgroundColor"] =
                    (SolidColorBrush)new BrushConverter().ConvertFromString(modpackHeader.BackgroundColor);
            }

            if (!string.IsNullOrEmpty(modpackHeader.PrimaryForegroundColor))
            {
                Application.Current.Resources["PrimaryForegroundColor"] = (SolidColorBrush)new BrushConverter().ConvertFromString(modpackHeader.PrimaryForegroundColor);
            }

            if (!string.IsNullOrEmpty(modpackHeader.SecondaryForegroundColor))
            {
                Application.Current.Resources["SecondaryForegroundColor"] = (SolidColorBrush)new BrushConverter().ConvertFromString(modpackHeader.SecondaryForegroundColor);
            }

            if (!string.IsNullOrEmpty(modpackHeader.HeaderImage))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                bitmapImage.UriSource = new Uri(modpackHeader.HeaderImage);

                bitmapImage.EndInit();

                Application.Current.Resources["HeaderImage"] = bitmapImage;
            }


        }

        #region Window Manipulation Code
        private static void CloseWindow(Window window)
        {
            window.Close();
        }

        private static void MinimizeWindow(Window window)
        {
            window.WindowState = WindowState.Minimized;
        }

        private static void MoveWindow(Window window)
        {
            try
            {
                window.DragMove();
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        #endregion
    }
}
