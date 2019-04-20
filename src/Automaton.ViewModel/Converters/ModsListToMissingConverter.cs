using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Autofac;
using Automaton.Model.Install;

namespace Automaton.ViewModel.Converters
{
    public class ModsListToMissingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var modsList = value as List<ExtendedMod>;

            if (modsList == null || !modsList.Any())
            {
                return modsList;
            }

            return modsList.Where(x => !x.IsValidationComplete).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
