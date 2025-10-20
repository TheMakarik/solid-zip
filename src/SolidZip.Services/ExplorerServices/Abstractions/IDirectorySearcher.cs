namespace SolidZip.Services.ExplorerServices.Abstractions;

public interface IDirectorySearcher
{
    public FileEntity GetDirectory(string path, string pattern);
    
}