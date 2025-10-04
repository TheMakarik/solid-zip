using SolidZip.Converters.Abstractions;

namespace SolidZip.Converters;

[ValueConversion(typeof(string), typeof(string))]
public sealed class PathToNameConvertor : OneWayConvertor
{
    private readonly IOptions<ExplorerOptions> _explorerOptions =
        Ioc.Default.GetRequiredService<IOptions<ExplorerOptions>>();
    
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
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
    
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}