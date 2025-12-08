namespace SolidZip.Core.Options;

public class LocalizationOptions
{
    public required Dictionary<string, CultureInfo> SupportedCultures { get; set; }
}