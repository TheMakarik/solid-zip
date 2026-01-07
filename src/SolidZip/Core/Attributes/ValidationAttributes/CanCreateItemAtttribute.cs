namespace SolidZip.Core.Attributes.ValidationAttributes;

public class CanCreateItemAttribute : ValidationAttribute
{
    private static readonly  string[] _reservedNames = {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    private const int WindowsItemMaxLength = 255;
    
    private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars()
        .Concat(Path.GetInvalidPathChars())
        .Distinct()
        .ToArray();

    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {

        var folderName = value.ToString() ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(folderName))
            return new ValidationResult(nameof(CannotCreateItemProblems.Empty));

        if (_invalidChars.Any(invalidChar => folderName.Contains(invalidChar)))
            return new ValidationResult(nameof(CannotCreateItemProblems.InvalidChars));

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(folderName);
        if (_reservedNames.Contains(fileNameWithoutExtension.ToUpperInvariant()))
            return new ValidationResult(nameof(CannotCreateItemProblems.ReservedString));
        
        if (folderName.EndsWith("."))
            return new ValidationResult(nameof(CannotCreateItemProblems.DotEnding));
        
        
        return folderName.Length > WindowsItemMaxLength 
            ? new ValidationResult(nameof(CannotCreateItemProblems.TooLong))
            : ValidationResult.Success;
    }

}