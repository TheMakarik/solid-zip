namespace SolidZip.Views.Converters;

[ValueConversion(typeof(string[]), typeof(ImageSource))]
public sealed class NotNullImageSourceMultiValueConverter(PathToImageSourceConvertor converter) : IMultiValueConverter
{
    public object? Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values.First(@object => @object is not null);
        return value is not string path
            ? null
            : converter.Convert(path, typeof(Type), parameter, CultureInfo.CurrentCulture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}