namespace SolidZip.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ArchiveExtensionsAttribute(params string[] extensions) : Attribute
{
    public string[] Extensions { get; } = extensions;
}