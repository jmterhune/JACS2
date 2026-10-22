using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CourtCounsel.Desktop.Converters;

// Lets a group of RadioButtons bind IsChecked to one shared string/enum
// property: IsChecked="{Binding StatusFilter, Converter={StaticResource EqualsToBoolean}, ConverterParameter=Active}"
public class EqualsToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        Equals(value?.ToString() ?? "", parameter?.ToString() ?? "");

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? parameter : Binding.DoNothing;
}
