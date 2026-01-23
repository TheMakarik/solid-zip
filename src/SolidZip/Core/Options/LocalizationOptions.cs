namespace SolidZip.Core.Options;

public sealed class LocalizationOptions
{
    public required Dictionary<string, CultureInfo> SupportedCultures { get; set; }
}