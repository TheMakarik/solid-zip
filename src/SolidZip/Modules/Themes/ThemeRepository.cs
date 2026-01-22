namespace SolidZip.Modules.Themes;

public class ThemeRepository(
    RetrySystem retrySystem,
    PathFormatter pathsFormatter,
    PathsCollection paths,
    ILogger<ThemeRepository> logger) : IThemeRepository
{
    private const int MaxRetry = 5;
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);

    public async Task UpdateOrCreateAsync(Theme theme, string themeName)
    {
        var path = GetThemePath(themeName);
        if (File.Exists(path))
            await CreateFileAsync(theme, path);
        else
            await UpdateFileAsync(theme, path);
    }

    public async Task CreateAsync(Theme theme, string path)
    {
        await CreateFileAsync(theme, path);
    }

    public async Task RemoveAsync(string themeName)
    {
        var path = GetThemePath(themeName);
        logger.LogInformation("Deleting theme: {path}", path);
        await retrySystem.RetryWithDelayAsync<InvalidOperationException>(
            new Task(() => File.Delete(path)), Delay, MaxRetry);
    }

    public async Task<Theme?> GetAsync(string themeName)
    {
        var path = GetThemePath(themeName);

        if (!File.Exists(path))
        {
            logger.LogWarning("Trying to load theme from unexisting path: {path}", path);
            return null;
        }


        logger.LogInformation("Getting theme from: {path}", path);
        return await retrySystem.RetryWithDelayAsync<InvalidOperationException, Theme?>(
            new Task<Theme?>(() =>
            {
                using var stream = new FileStream(path, FileMode.Open);
                var serializer = new XmlSerializer(typeof(Theme));
                return serializer.Deserialize(stream) as Theme?;
            }), Delay, MaxRetry);
    }

    private string GetThemePath(string themeName)
    {
        return pathsFormatter.GetThemePath(themeName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task UpdateFileAsync(Theme theme, string path)
    {
        EnsureThemeFolderExists();
        logger.LogInformation("Updating theme {path} to {theme}", path, theme);
        await SerializeThemeAsync(theme, path, FileMode.Truncate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task CreateFileAsync(Theme theme, string path)
    {
        EnsureThemeFolderExists();
        logger.LogInformation("Creating theme {theme} on {path}", theme, path);
        await SerializeThemeAsync(theme, path, FileMode.Create);
    }

    private async Task SerializeThemeAsync(Theme theme, string path, FileMode mode)
    {
        await retrySystem.RetryWithDelayAsync<InvalidOperationException>(
            new Task(() =>
            {
                using var stream = new FileStream(path, mode);
                var serializer = new XmlSerializer(typeof(Theme));
                serializer.Serialize(stream, theme);
            }), Delay, MaxRetry);
    }

    private void EnsureThemeFolderExists()
    {
        if (!Directory.Exists(paths.Themes))
            Directory.CreateDirectory(paths.Themes);
    }
}