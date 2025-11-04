namespace SolidZip.Views.Converters;

[ValueConversion(typeof(string), typeof(string))]
public sealed class PathToNameConvertor(IOptions<ExplorerOptions> explorerOptions) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path)
            return null;
        
        //Deeper directory name, by default is ".." so, if you try to get DirectoryInfo("..").Name
        //will be returned path to the assembly folder of the project
        if (path == explorerOptions.Value.DeeperDirectory)
            return path;
        
        return File.Exists(path) 
            ? Path.GetFileNameWithoutExtension(path) 
            : new DirectoryInfo(path).Name;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
    
}