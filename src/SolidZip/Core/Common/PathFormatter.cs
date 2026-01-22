namespace SolidZip.Core.Common;

public class PathFormatter(PathsCollection paths)
{
    public string GetThemePath(string themeName)
    {
        return Path.Combine(paths.Themes, themeName + ".xml");
    }
}