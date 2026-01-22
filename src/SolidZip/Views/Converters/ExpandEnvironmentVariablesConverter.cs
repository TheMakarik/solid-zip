namespace SolidZip.Views.Converters;

[ValueConversion(typeof(string), typeof(string))]
public sealed class ExpandEnvironmentVariablesConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
            return Environment.ExpandEnvironmentVariables(stringValue);
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}