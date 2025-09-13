namespace SolidZip.Model.Options;

public class ExplorerOptions
{
    public required string RootDirectory { get; init; }
    public required string[] RootDirectoryAdditionalContent { get; init; }
}