namespace SolidZip.Factories;

public sealed class ArchiveReaderFactory(IArchiveSupportedExtensions extensions, IServiceProvider services)
{
    public bool TryGetFactory(string path, [NotNullWhen(true)] out IArchiveReader? result)
    {
        //Now if i use .tar.{compressor} extension, I get only .{compressor} but if i try to open,
        //SharpCompress will read this .tar.{compressor} archive
        var extension = Path.GetExtension(path);

        if (extensions.Contains(extension))
        {
            result = services.GetRequiredKeyedService<IArchiveReader>(extension);
            result.SetPath(path);
            return true;
        }

        result = null;
        return false;
    }
}