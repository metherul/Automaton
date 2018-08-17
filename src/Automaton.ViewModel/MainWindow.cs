using System;
using System.ComponentModel;
using System.Windows;
using Automaton.Model.Utility;
using Automaton.ViewModel.Controllers;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class MainWindow : ViewController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand<Window> CloseWindowCommand { get; set; }
        public RelayCommand<Window> MinimizeWindowCommand { get; set; }
        public RelayCommand<Window> MoveWindowCommand { get; set; }

        public int CurrentTransitionerIndex { get; set; } = 0;

        public MainWindow()
        {
            // Initialize relaycommand bindings
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
            MinimizeWindowCommand = new RelayCommand<Window>(MinimizeWindow);
            MoveWindowCommand = new RelayCommand<Window>(MoveWindow);

            // Initialize event handlers
            Modpack.ModpackLoadedEvent += ModpackLoaded;
            ViewIndexChangedEvent += ViewIndexUpdate;
        }

        public void ModpackLoaded()
        {
            // Modpack has been loaded, so increment the current view index
            IncrementCurrentViewIndex();

            ThemeController.ApplyTheme();
        }

        private void ViewIndexUpdate(int index)
        {
            CurrentTransitionerIndex = index;
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
