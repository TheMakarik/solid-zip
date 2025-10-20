using SolidZip.Services.Validators.Abstractions;

namespace SolidZip.Services.Validators;

public class GlobalValidator : IValidator
{
    public bool IsLogicalDrive(string path)
    {
        return path is [_, ':', _] && path[2] == Path.DirectorySeparatorChar;
    }
}