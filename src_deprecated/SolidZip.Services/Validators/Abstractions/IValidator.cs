namespace SolidZip.Services.Validators.Abstractions;

public interface IValidator
{
    public bool IsLogicalDrive(string path);
}