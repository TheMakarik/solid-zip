namespace SolidZip.Views.MarkupExtensions;

public sealed class ConvertersExtension(Type converterType) : MarkupExtension
{
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return Ioc.Default.GetService(converterType);
    }
}