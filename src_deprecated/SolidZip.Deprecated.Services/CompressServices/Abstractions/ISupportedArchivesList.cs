namespace SolidZip.Services.CompressServices.Abstractions;

public interface ISupportedArchivesList
{
    public void AddArchiveExtension(string extension);
    public bool IsSupported(string extension);
}