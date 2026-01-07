namespace SolidZip.Core.Attributes.ValidationAttributes;

public class DirectoryExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string directory)
            throw new ArgumentException("value must be string");
        
        
        return Directory.Exists(directory) 
            ? ValidationResult.Success 
            : new ValidationResult(nameof(CannotCreateItemProblems.AlreadyExists));
    }
}