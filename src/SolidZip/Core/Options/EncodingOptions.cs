namespace SolidZip.Core.Options;

public sealed class EncodingOptions
{
    public required string[] EncodingErrorsChars { get; set; }
    public required string[] EncodingBeforeAutodetect { get; set; }
}