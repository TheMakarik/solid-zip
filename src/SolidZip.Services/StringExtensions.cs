namespace SolidZip.Services;

public static class StringExtensions
{
    public static string ReplaceSeparatorsToAlt(this string value)
    {
        return value.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
    
    public static string ReplaceSeparatorsToDefault(this string value)
    {
        return value.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    }

}