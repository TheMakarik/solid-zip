namespace SolidZip.Domain.Entities;

public struct UserSettings
{
    public required CultureInfo CurrentCulture { get; set; }
    public required ExplorerElementsViewType ExplorerElementsViewType { get; set; }
}