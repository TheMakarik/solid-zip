namespace SolidZip.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AssociatedArchiveCreatorAttribute(Type creatorType) : Attribute
{
    public Type CreatorType { get; } = creatorType;
}