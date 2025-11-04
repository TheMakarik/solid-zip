namespace SolidZip.Core.Contracts.AppData;

public interface IThemeFileLoader
{
    public void CreateFile(string path, Theme theme);
}