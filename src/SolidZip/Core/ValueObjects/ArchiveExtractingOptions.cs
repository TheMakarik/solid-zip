namespace SolidZip.Core.ValueObjects;

public struct ArchiveExtractingOptions
{
    public bool Override { get; set; }
    public bool CreateExtractionDirectory { get; set; }
    public bool PreserveFileTime { get; set; }
    public bool PreserveAttributes { get; set; }
}