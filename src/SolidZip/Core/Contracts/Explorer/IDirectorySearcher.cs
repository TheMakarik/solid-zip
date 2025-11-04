namespace SolidZip.Core.Contracts.Explorer;

public interface IDirectorySearcher
{
    public string Search(string path, string pattern);
}