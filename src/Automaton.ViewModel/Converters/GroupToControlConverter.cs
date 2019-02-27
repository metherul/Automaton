using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Automaton.ViewModel.Converters
{
    public class GroupToControlConverter : IValueConverter
    {
        //public GroupToControlConverter(ISetupAssistant setupAssistant)
        //{
        //    _setupAssistant = setupAssistant;
        //}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var groupControls = value as List<GroupControl>;
            //var controlList = new List<object>();
            //var stackPanel = new StackPanel();

            //// TODO: Need to reduce duplicated code in this converter. Nothing insanely important, but it doesn't look too nice.
            //foreach (var groupControl in groupControls)
            //{
            //    // Convert control object to a WPF equivalent
            //    if (groupControl.ControlType == Types.ControlType.CheckBox)
            //    {
            //        var control = new CheckBox()
            //        {
            //            Content = groupControl.ControlText,
            //            CommandParameter = groupControl,
            //            HorizontalAlignment = HorizontalAlignment.Left
            //        };

            //        control.Checked += _setupAssistant.ControlChecked;
            //        control.Unchecked += _setupAssistant.ControlUnchecked;
            //        control.MouseEnter += _setupAssistant.ControlHover;

            //        control.Foreground = (SolidColorBrush)Application.Current.Resources["FontColor"];
            //        control.Background = (SolidColorBrush)Application.Current.Resources["AssistantControlColor"];

            //        control.ToolTip = groupControl.ControlHoverDescription;

            //        control.IsChecked = groupControl.IsControlChecked ?? false;

            //        if ((bool)control.IsChecked)
            //        {
            //            _setupAssistant.ControlChecked(control, new RoutedEventArgs());
            //        }

            //        stackPanel.Children.Add(control);
            //    }
            //    else if (groupControl.ControlType == Types.ControlType.RadioButton)
            //    {
            //        var control = new RadioButton()
            //        {
            //            Content = groupControl.ControlText,
            //            CommandParameter = groupControl,
            //            HorizontalAlignment = HorizontalAlignment.Left
            //        };

            //        control.Checked += _setupAssistant.ControlChecked;
            //        control.Unchecked += _setupAssistant.ControlUnchecked;
            //        control.MouseEnter += _setupAssistant.ControlHover;

            //        control.Foreground = (SolidColorBrush)Application.Current.Resources["FontColor"];
            //        control.Background = (SolidColorBrush)Application.Current.Resources["AssistantControlColor"];

            //        control.ToolTip = groupControl.ControlHoverDescription;

            //        control.IsChecked = groupControl.IsControlChecked ?? false;

            //        if ((bool)control.IsChecked)
            //        {
            //            _setupAssistant.ControlChecked(control, new RoutedEventArgs());
            //        }

            //        stackPanel.Children.Add(control);
            //    }
            // }

            //controlList.Add(stackPanel);

            //return controlList;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
