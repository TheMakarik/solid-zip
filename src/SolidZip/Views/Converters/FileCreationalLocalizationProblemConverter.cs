namespace SolidZip.Views.Converters;

public sealed class FileCreationalLocalizationProblemConverter(StrongTypedLocalizationManager localization) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var error = (value as ReadOnlyObservableCollection<ValidationError>)?.FirstOrDefault()?.ErrorContent.ToString();
        var couldParse = Enum.TryParse<CannotCreateItemProblems>(error ?? string.Empty, out var result);
        if (!couldParse)
            return null;

        return result switch
        {
            CannotCreateItemProblems.AlreadyExists => localization.FileAlreadyExists,
            CannotCreateItemProblems.DotEnding => localization.FileMustNotEndWithDot,
            CannotCreateItemProblems.Empty => localization.NameCannotBeEmpty,
            CannotCreateItemProblems.TooLong => localization.NameTooLong,
            CannotCreateItemProblems.InvalidChars => localization.NameUsingInvalidCharacters,
            CannotCreateItemProblems.ReservedString => localization.FileUsingReservedStrings,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}