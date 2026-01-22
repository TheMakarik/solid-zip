namespace SolidZip.Modules.Archiving;

public sealed class ZipArchiveCreator : IZipArchiveCreator
{
    public async Task CreateZipArchiveAsync(ZipArchiveCreationalOptions options, IProgress<double> progress)
    {
        if (string.IsNullOrWhiteSpace(options.ZipFileName))
            throw new InvalidOperationException("ZipFileName is required");

        if (File.Exists(options.ZipFileName))
            throw new InvalidOperationException($"Zip archive {options.ZipFileName} already exists");

        await CreateNewZipArchiveAsync(options, progress);
    }

    private async Task CreateNewZipArchiveAsync(ZipArchiveCreationalOptions options, IProgress<double> progress)
    {
        using var zip = new ZipFile();
        zip.AddProgress += (sender, args) => { progress.Report(args.EntriesTotal); };
        zip.EmitTimesInWindowsFormatWhenSaving = true;
    }
}