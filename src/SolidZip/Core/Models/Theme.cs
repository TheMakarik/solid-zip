namespace SolidZip.Core.Models;

public sealed class Theme
{
    public required string Name { get; set; }
    public required string PrimaryColorHex { get; set; }
    public required string ForegroundColorHex { get; set; }
    public required string ForegroundHoverColorHex { get; set; }
    public required string BackgroundColorHex { get; set; }
}