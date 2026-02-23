namespace SolidZip.Views.Converters;

public class TitleFormatConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {

        if (values is not [string, string] formatStrings)
            return null;
        
        return values[0] + " - " + formatStrings[1];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}