using Automaton.Model.Extensions;
using Automaton.Model.ModpackBase;
using Automaton.ViewModel.Controllers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class SetupAssistant : ISetupAssistant, INotifyPropertyChanged
    {
        public IViewController _viewController;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Group> SetupAssistantGroup { get; set; }

        public RelayCommand IncrementCurrentViewIndexCommand { get; set; }

        public string ImagePath { get; set; }
        public string Description { get; set; }

        public SetupAssistant(IViewController viewController)
        {
            _viewController = viewController;

            IncrementCurrentViewIndexCommand = new RelayCommand(_viewController.IncrementCurrentViewIndex);
        }

        private void ModpackLoaded()
        {
            if (!Model.Instance.AutomatonInstance.ModpackHeader.ContainsSetupAssistant) return;

            var setupAssistant = Model.Instance.AutomatonInstance.ModpackHeader.SetupAssistant;

            SetupAssistantGroup = new ObservableCollection<Group>(setupAssistant.ControlGroups);

            ImagePath = setupAssistant.DefaultImage;
            Description = setupAssistant.DefaultDescription;
        }

        private static void RouteControlActionEvent(dynamic sender, FlagEventType flagEventType)
        {
            var controlObject = (GroupControl)sender.CommandParameter;
            var matchingControlActions = controlObject.ControlActions
                .Where(x => x.FlagEvent == flagEventType);

            if (matchingControlActions.NullAndAny())
            {
                foreach (var action in matchingControlActions)
                {
                    Model.Instance.FlagInstance.AddOrModifyFlag(action.FlagName, action.FlagValue, action.FlagAction);
                }
            }
        }

        public static void ControlChecked(dynamic sender, RoutedEventArgs e)
        {
            RouteControlActionEvent(sender, FlagEventType.Checked);
        }

        public static void ControlUnchecked(dynamic sender, RoutedEventArgs e)
        {
            RouteControlActionEvent(sender, FlagEventType.UnChecked);
        }

        public static void ControlHover(dynamic sender, RoutedEventArgs e)
        {
            var controlObject = (GroupControl)sender.CommandParameter;

            // Terrible code
            if (!string.IsNullOrEmpty(controlObject.ControlHoverImage))
            {
            }

            if (!string.IsNullOrEmpty(controlObject.ControlHoverImage))
            {
            }
        }
    }

    public class GroupToControlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var groupControls = value as List<GroupControl>;
            var controlList = new List<object>();
            var stackPanel = new StackPanel();

            // TODO: Need to reduce duplicated code in this converter. Nothing insanely important, but it doesn't look too nice.
            foreach (var groupControl in groupControls)
            {
                // Convert control object to a WPF equivalent
                if (groupControl.ControlType == ControlType.CheckBox)
                {
                    var control = new CheckBox()
                    {
                        Content = groupControl.ControlText,
                        CommandParameter = groupControl,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    control.Checked += SetupAssistant.ControlChecked;
                    control.Unchecked += SetupAssistant.ControlUnchecked;
                    control.MouseEnter += SetupAssistant.ControlHover;

                    control.Foreground = (SolidColorBrush)Application.Current.Resources["FontColor"];
                    control.Background = (SolidColorBrush)Application.Current.Resources["AssistantControlColor"];

                    control.ToolTip = groupControl.ControlHoverDescription;

                    control.IsChecked = groupControl.IsControlChecked ?? false;

                    if ((bool)control.IsChecked)
                    {
                        SetupAssistant.ControlChecked(control, new RoutedEventArgs());
                    }

                    stackPanel.Children.Add(control);
                }
                else if (groupControl.ControlType == ControlType.RadioButton)
                {
                    var control = new RadioButton()
                    {
                        Content = groupControl.ControlText,
                        CommandParameter = groupControl,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    control.Checked += SetupAssistant.ControlChecked;
                    control.Unchecked += SetupAssistant.ControlUnchecked;
                    control.MouseEnter += SetupAssistant.ControlHover;

                    control.Foreground = (SolidColorBrush)Application.Current.Resources["FontColor"];
                    control.Background = (SolidColorBrush)Application.Current.Resources["AssistantControlColor"];

                    control.ToolTip = groupControl.ControlHoverDescription;

                    control.IsChecked = groupControl.IsControlChecked ?? false;

                    if ((bool)control.IsChecked)
                    {
                        SetupAssistant.ControlChecked(control, new RoutedEventArgs());
                    }

                    stackPanel.Children.Add(control);
                }
            }

            controlList.Add(stackPanel);

            return controlList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}