using System;
using System.Globalization;
using System.Windows.Data;
using SolidZip.Localization;

namespace SolidZip.Views.Converters;

[ValueConversion(typeof(Enum), typeof(string))]
public class EnumToLocalizedStringConverter(StrongTypedLocalizationManager localization) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return string.Empty;
            
        var enumValue = value.ToString();
        
        var propertyName = enumValue;
        var property = typeof(StrongTypedLocalizationManager).GetProperty(propertyName);
        
        if (property is not null)
        {
            return property.GetValue(localization)?.ToString() ?? enumValue;
        }
        
        return enumValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}