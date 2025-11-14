namespace SolidZip.Modules.Themes;

public class ThemeSetter(Action<string, string> setThemeAction) : IThemeSetter
{
    public void SetTheme(Theme theme)
    {
        setThemeAction(theme.PrimaryColorHex, nameof(theme.PrimaryColorHex).Replace("Hex", "Brush"));
        setThemeAction(theme.BackgroundColorHex, nameof(theme.BackgroundColorHex).Replace("Hex", "Brush"));
        setThemeAction(theme.ForegroundColorHex, nameof(theme.ForegroundColorHex).Replace("Hex", "Brush"));
        setThemeAction(theme.ForegroundHoverColorHex, nameof(theme.ForegroundHoverColorHex).Replace("Hex", "Brush"));
        setThemeAction(theme.WarningColorHex, nameof(theme.WarningColorHex).Replace("Hex", "Brush"));
    }
}