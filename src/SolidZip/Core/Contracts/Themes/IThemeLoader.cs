namespace SolidZip.Core.Contracts.Themes;

public interface IThemeLoader
{
    public ValueTask LoadAsync(string themeName);
}