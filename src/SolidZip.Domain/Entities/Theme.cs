namespace SolidZip.Domain.Entities;

public class Theme
{
    public required string PrimaryColorHex { get; set; }
    public required string ForegroundColorHex { get; set; }
    public required string ForegroundHoverColorHex { get; set; }
    public required string BackgroundColorHex { get; set; }
    public required string WarningPopupColorHex { get; set; }
}