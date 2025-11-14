namespace SolidZip.Core.Contracts.Themes;

public interface IThemeRepository
{
    public Task UpdateOrCreateAsync(Theme theme, string themeName);
    public Task CreateAsync(Theme theme, string path);
    public Task RemoveAsync(string themeName);
    public Task<Theme?> GetAsync(string themeName);
}