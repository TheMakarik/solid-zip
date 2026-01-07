namespace SolidZip.Views.Converters;

[ValueConversion(typeof(string), typeof(string))]
public class DirectoryCreationalLocalizationProblemConverter(StrongTypedLocalizationManager localization) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
       var error = (value as ReadOnlyObservableCollection<ValidationError>)?.FirstOrDefault()?.ErrorContent.ToString();
       var couldParse = Enum.TryParse<CannotCreateItemProblems>(error ?? string.Empty, out var result);
       if (!couldParse)
           return null;
       
       return result switch
       {
            CannotCreateItemProblems.AlreadyExists => localization.DirectoryAlreadyExists,
            CannotCreateItemProblems.DotEnding => localization.DirectoryMustNotEndWithDot,
            CannotCreateItemProblems.Empty => localization.NameCannotBeEmpty,
            CannotCreateItemProblems.TooLong => localization.NameTooLong,
            CannotCreateItemProblems.InvalidChars => localization.NameUsingInvalidCharacters,
            CannotCreateItemProblems.ReservedString => localization.DirectoryUsingReservedStrings,
            _ => throw new ArgumentOutOfRangeException()
       };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}