using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace SolidZip.Converters;

[ValueConversion(typeof(string), typeof(string))]
public class PathToFileNameConverter : IValueConverter
{

    public static PathToFileNameConverter Instance { get; } = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var path = value as string;

        if (path is null)
            return null;

        if (Directory.Exists(path))
            return Path.GetDirectoryName(path);
        return Path.GetFileName(path);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}