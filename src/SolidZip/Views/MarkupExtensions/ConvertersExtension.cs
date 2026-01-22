namespace SolidZip.Views.MarkupExtensions;

public sealed class ConvertersExtension : MarkupExtension
{
    public string? Converter { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return Ioc.Default.GetRequiredService(Converter ?? string.Empty);
    }
}