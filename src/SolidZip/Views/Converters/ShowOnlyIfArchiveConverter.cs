namespace SolidZip.Views.Converters;

[ValueConversion(typeof(void), typeof(Visibility))]
public class ShowOnlyIfArchiveConverter(IFileSystemStateMachine fileSystemStateMachine) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return fileSystemStateMachine.GetState() == FileSystemState.Archive
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}