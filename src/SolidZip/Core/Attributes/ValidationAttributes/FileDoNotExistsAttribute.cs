namespace SolidZip.Core.Attributes.ValidationAttributes;

public sealed class FileDoNotExistAttribute(string currentDirectoryFieldName) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string fileName)
            throw new ArgumentException("value must be string");

        var directoryPath = validationContext.ObjectInstance.GetType()?
                                .GetField(currentDirectoryFieldName,
                                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField |
                                    BindingFlags.Public)?
                                .GetValue(validationContext.ObjectInstance) as string ??
                            throw new ArgumentException($"{nameof(currentDirectoryFieldName)} must be a string");

        var filePath = Path.Combine(directoryPath, fileName);

        if (fileName == string.Empty)
            return ValidationResult.Success; //Next validator will be returned mistake 

        return !FileExists(filePath)
            ? ValidationResult.Success
            : new ValidationResult(nameof(CannotCreateItemProblems.AlreadyExists));
    }

    private bool FileExists(string filePath)
    {
        return File.Exists(filePath) || Directory.Exists(filePath);
    }
}