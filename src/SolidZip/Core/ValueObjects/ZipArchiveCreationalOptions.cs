namespace SolidZip.Core.ValueObjects;

public sealed class ZipArchiveCreationalOptions
{
    private string _zipFileName = string.Empty;
    
    public required string OutputDirectory { get; set; }

    public required string ZipFileName
    {
        get => _zipFileName;
        set => _zipFileName = value.AddExtensionIfNotAdded(".zip");
    }

    public List<string> FilesToAdd { get; } = [];
    public ZipEncryption ZipEncryption { get; set; }
    public string? Password { get; set; }
}