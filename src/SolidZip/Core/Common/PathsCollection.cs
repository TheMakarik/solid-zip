namespace SolidZip.Core.Common;

public class PathsCollection(IOptions<PathsOptions> options)
{
    private readonly PathsOptions _options = options.Value;

    public string AppData => Environment.ExpandEnvironmentVariables(_options.AppData);
    public string IconPath => Environment.ExpandEnvironmentVariables(_options.IconPath);
    public string UserData => Environment.ExpandEnvironmentVariables(_options.UserData);
    public string Logging => Environment.ExpandEnvironmentVariables(_options.Logging);
    public string Themes => Environment.ExpandEnvironmentVariables(_options.Themes);

    public string[] Plugins => _options.PluginsPath
        .Select(p => Environment.ExpandEnvironmentVariables(p) ?? string.Empty).ToArray();

    public virtual string Modules => Environment.ExpandEnvironmentVariables(_options.ModulesPath);
    public string UnknownLanguageIcon => Environment.ExpandEnvironmentVariables(_options.UnknownLanguageIcon);
    public string LanguageIcons => Environment.ExpandEnvironmentVariables(_options.LanguageIcons);
}