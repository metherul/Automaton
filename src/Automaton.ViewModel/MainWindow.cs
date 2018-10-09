using Automaton.Model.Instance;
using Automaton.ViewModel.Controllers;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Windows;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class MainWindow : IMainWindow, INotifyPropertyChanged
    {
        private IViewController _viewController;

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand<Window> CloseWindowCommand { get; set; }
        public RelayCommand<Window> MinimizeWindowCommand { get; set; }
        public RelayCommand<Window> MoveWindowCommand { get; set; }

        public int CurrentTransitionerIndex { get; set; } = 0;

        public MainWindow(IViewController viewController)
        {
            _viewController = viewController;

            // Initialize event handlers
            _viewController.ViewIndexChangedEvent += ViewIndexUpdate;

            // Initialize relaycommand bindings
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
            MinimizeWindowCommand = new RelayCommand<Window>(MinimizeWindow);
            MoveWindowCommand = new RelayCommand<Window>(MoveWindow);

            // Initialize the instance
            AutomatonInstance.InitializeInstance();
        }

        private void ViewIndexUpdate(object sender, int currentIndex)
        {
            CurrentTransitionerIndex = currentIndex;
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

        #endregion Window Manipulation Code
    }
}