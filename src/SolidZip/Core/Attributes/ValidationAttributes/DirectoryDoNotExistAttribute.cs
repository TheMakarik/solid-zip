namespace SolidZip.Core.Attributes.ValidationAttributes;

public class DirectoryDoNotExistAttribute(string currentDirectoryFieldName) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string directoryName)
            throw new ArgumentException("value must be string");
        var directoryPath = validationContext.ObjectInstance.GetType()?
            .GetField(currentDirectoryFieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public)?
            .GetValue(validationContext.ObjectInstance) as string ?? throw new ArgumentException($"{nameof(currentDirectoryFieldName)} must be a string");
        
        var directory = Path.Combine(directoryPath, directoryName);
        
        if(directoryName == string.Empty)
            return ValidationResult.Success; //Next validator will be returned mistake 
        
        return !DirectoryExists(directory)
            ? ValidationResult.Success 
            : new ValidationResult(nameof(CannotCreateItemProblems.AlreadyExists));
    }

    private bool DirectoryExists(string directory)
    {
        return (Directory.Exists(directory) && Uri.TryCreate(directory, UriKind.Absolute, out _));
    }
}