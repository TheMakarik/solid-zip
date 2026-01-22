namespace SolidZip.Views.ValueObjects;

public record struct MultiSelectorItemInfo(
    object Control,
    bool IsSelected,
    Point TopLeft,
    Point TopRight,
    Point BottomLeft,
    Point BottomRight);