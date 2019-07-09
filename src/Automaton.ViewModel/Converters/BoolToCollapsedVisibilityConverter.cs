using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Automaton.ViewModel.Converters
{
    public class BoolToCollapsedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter.ToString() == "Inverse")
            {
                if ((bool)value)
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }

            if ((bool)value)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
