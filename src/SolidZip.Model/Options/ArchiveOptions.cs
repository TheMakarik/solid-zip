namespace SolidZip.Model.Options;

public class ArchiveOptions
{
    public required string ArchiveConfigurationPath { get; set; }
    public required ArchiveConfiguration DefaultConfiguration { get; set; }
}