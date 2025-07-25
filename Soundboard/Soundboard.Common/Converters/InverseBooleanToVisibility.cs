using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Soundboard.Common.Converters;

public class InverseBooleanToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var visibility = (Visibility)value;
        switch (visibility)
        {
            case Visibility.Visible: return false;
            case Visibility.Collapsed: return true;
            default: throw new ArgumentException($"{nameof(value)} must be either {nameof(Visibility.Visible)} or {nameof(Visibility.Hidden)}.", nameof(value));
        }
    }
}
