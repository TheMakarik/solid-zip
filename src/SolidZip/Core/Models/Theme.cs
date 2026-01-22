namespace SolidZip.Core.Models;

public struct Theme
{
    public required string Name { get; set; }
    public required string PrimaryColorHex { get; set; }
    public required string ForegroundColorHex { get; set; }
    public required string ForegroundHoverColorHex { get; set; }
    public required string BackgroundColorHex { get; set; }
    public required string WarningColorHex { get; set; }
    public required ThemeMetadata ThemeMetadata { get; set; }
}