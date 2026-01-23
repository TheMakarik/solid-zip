namespace SolidZip.Core.Options;

public sealed class PathsOptions
{
    public required string AppData { get; set; }
    public required string UserData { get; set; }
    public required string Logging { get; set; }
    public required string LogsArchive { get; set; }
    public required string IconPath { get; set; }
    public required string LanguageIcons { get; set; }
    public required string UnknownLanguageIcon { get; set; }
    public required string Themes { get; set; }

    public required string ModulesPath { get; set; }
    public required string[] PluginsPath { get; set; }
}