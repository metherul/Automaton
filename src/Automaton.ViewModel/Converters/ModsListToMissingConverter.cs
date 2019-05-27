using System;
using System.Globalization;
using System.Windows.Data;

namespace Automaton.ViewModel.Converters
{
    public class ModsListToMissingConverter : IValueConverter // Experimental implementation, might be able to be removed 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
