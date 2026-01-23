namespace SolidZip.Core.Models;

public sealed class UserData
{
    public required CultureInfo CurrentCulture { get; set; }
    public required string ExplorerElementsView { get; set; }
    public required string CurrentTheme { get; set; }
    public required FileSizeMeasurement FileSizeMeasurement { get; set; }
    public required bool AttachPluginsConsole { get; set; }
    public required List<string> RootDirectoryAdditionalContent { get; set; }
    [ValueRange(15, 30)] public int ExplorerElementsHeight { get; set; }
    public required bool ShowHiddenDirectories { get; set; }
}