namespace SolidZip.Services.ExplorerServices.Abstractions;

public interface IExplorer
{
    public (IEnumerable<FileEntity> Entities, ExplorerResult Result) GetDirectoryContent(FileEntity entity);
}