namespace SolidZip.Deprecated.Converters;

[ValueConversion(typeof(string[]), typeof(ImageSource))]
public class NotNullImageSourceMultiValueConverter : MarkupExtension, IMultiValueConverter
{
    public object? Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values.First(@object => @object is not null);
        if (value is not string path)
            return null;

        return PathToImageSourceConvertor.Instance.Convert(path, typeof(Type), null, CultureInfo.CurrentCulture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return this; 
    }
}