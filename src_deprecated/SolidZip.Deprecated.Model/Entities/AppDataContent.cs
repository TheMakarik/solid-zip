using SolidZip.Deprecated.Model.Enums;

namespace SolidZip.Deprecated.Model.Entities;

public struct AppDataContent
{
    public ExplorerElementsView ExplorerElementsView { get; set; }
    public string ThemeName { get; set; }
    public CultureInfo CurrentCulture { get; set; }
    public FileSizeMeasurement FileSizeMeasurement { get; set; }
}