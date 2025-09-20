namespace SolidZip.Converters;

[ValueConversion(typeof(string), typeof(string))]
public class PathToNameConvertor : MarkupExtension, IValueConverter
{
    private readonly IOptions<ExplorerOptions> _explorerOptions =
        Ioc.Default.GetRequiredService<IOptions<ExplorerOptions>>();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path)
            return null;
        
        //Deeper directory name, by default is ".." so, if you try to get DirectoryInfo("..").Name
        //will be returned path to the assembly folder of the project
        if (path == _explorerOptions.Value.DeeperDirectoryName)
            return path;
        
        return File.Exists(path) 
            ? Path.GetFileNameWithoutExtension(path) 
            : new DirectoryInfo(path).Name;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}