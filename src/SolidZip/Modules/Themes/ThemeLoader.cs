namespace SolidZip.Modules.Themes;

public sealed class ThemeLoader(
    IThemeSetter themeSetter,
    IUserJsonManager userJson,
    IThemeRepository themeRepository,
    ILogger<ThemeLoader> logger,
    PathFormatter pathFormatter,
    IOptions<DefaultOptions> defaults) : IThemeLoader
{
    public async ValueTask LoadAsync(string themeName)
    {
        var path = pathFormatter.GetThemePath(themeName);

        if (themeName == defaults.Value.Theme.Name)
        {
            LoadDefaultTheme(path);
            return;
        }

        if (File.Exists(path))
        {
            await LoadThemeAsync(path);
        }
        else
        {
            logger.LogError(
                "{path} is not exists, but tried to load, possible file deleting, switching to the default theme",
                path);
            userJson.ChangeThemeName(defaults.Value.Theme.Name);
            LoadDefaultTheme(path);
        }
    }

    private void LoadDefaultTheme(string path)
    {
        themeSetter.SetTheme(defaults.Value.Theme);
        if (!File.Exists(path))
            themeRepository.CreateAsync(defaults.Value.Theme, path);
    }

    private async Task LoadThemeAsync(string path)
    {
        var theme = await themeRepository.GetAsync(path);

        if (theme.HasValue)
        {
            themeSetter.SetTheme(theme.Value);
            return;
        }

        logger.LogError("{path} theme is corrupted, switching to the default theme", path);
        userJson.ChangeThemeName(defaults.Value.Theme.Name);
    }
}