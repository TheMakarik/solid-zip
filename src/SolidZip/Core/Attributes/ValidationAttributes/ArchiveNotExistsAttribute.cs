namespace SolidZip.Core.Attributes.ValidationAttributes;

public class ArchiveNotExistsAttribute(string archivePathPropertyName) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var archiveName = value as string ?? throw new ArgumentException("value must be string");
        var archivePath = validationContext.ObjectType.GetProperty(archivePathPropertyName)
            ?.GetValue(validationContext.ObjectInstance, null) as string ?? throw new ArgumentNullException(archivePathPropertyName);
        
        var path = Path.Combine(archivePath, archiveName);
        
        return File.Exists(path)
            ? ValidationResult.Success 
            : new ValidationResult(ErrorMessage);
    }
}