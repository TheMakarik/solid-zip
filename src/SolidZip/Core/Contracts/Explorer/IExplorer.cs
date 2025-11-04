namespace SolidZip.Core.Contracts.Explorer;

public interface IExplorer
{
    public ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetDirectoryContentAsync(FileEntity directory);
}