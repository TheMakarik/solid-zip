namespace SolidZip.Views.Converters;

[ValueConversion(typeof(void), typeof(Visibility))]
public class ShowOnlyIfInDirectoryConverter(IFileSystemStateMachine fileSystemStateMachine) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return fileSystemStateMachine.GetState() == FileSystemState.Directory
            ? Visibility.Visible 
            : Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}