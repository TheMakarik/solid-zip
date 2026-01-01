namespace SolidZip.Core.Contracts.Archiving;

public interface IZipArchiveCreator
{
    public Task CreateZipArchiveAsync(ZipArchiveCreationalOptions options, IProgress<double> progress);
}