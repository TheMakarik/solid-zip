namespace SolidZip.Model.Entities;

public struct AppDataContent
{
    public ExplorerElementsView ExplorerElementsView { get; set; }
    public string ThemeName { get; set; }
    public CultureInfo CurrentCulture { get; set; }
    public bool UseCustomIconsCollection { get; set; }
}