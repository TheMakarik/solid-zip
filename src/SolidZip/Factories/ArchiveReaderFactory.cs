namespace SolidZip.Factories;

public class ArchiveReaderFactory(IArchiveSupportedExtensions extensions, IServiceProvider services)
{
    public virtual bool TryGetFactory(string path, [NotNullWhen(true)] out IArchiveReader? result)
    {
        var extension = path.Contains(".tar.gz", StringComparison.InvariantCulture)
            ? ".tar.gz"
            : Path.GetExtension(path);

        if (extensions.Contains(extension))
        {
            result = services.GetRequiredKeyedService<IArchiveReader>(extension);
            return true;
        }

        result = null;
        return false;
    }
}