namespace SharpNeedle.Studio.Converters;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is bool val))
            return Visibility.Collapsed;

        return val ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is Visibility vis))
            throw new ArgumentException("Invalid argument");

        return vis == Visibility.Visible;
    }
}