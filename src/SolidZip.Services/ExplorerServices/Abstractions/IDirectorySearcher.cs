namespace SolidZip.Services.ExplorerServices.Abstractions;

public interface IDirectorySearcher
{
    public FileEntity Search(string path, string pattern);
    
}