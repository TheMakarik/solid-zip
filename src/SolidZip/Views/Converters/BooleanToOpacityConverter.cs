namespace SolidZip.Views.Converters;

[ValueConversion(typeof(bool), typeof(double))]
public class BooleanToOpacityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue) return 1.0;

        var invert = parameter?.ToString()?.ToLower() == "invert";
        var result = boolValue;
            
        if (invert)
            result = !result;
                
        return result ? 1.0 : 0.3; 

    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}