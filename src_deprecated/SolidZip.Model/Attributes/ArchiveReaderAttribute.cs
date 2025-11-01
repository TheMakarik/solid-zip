namespace SolidZip.Model.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ArchiveReaderAttribute(string extension) : Attribute
{
    public string Extension { get; init; } = extension;
}