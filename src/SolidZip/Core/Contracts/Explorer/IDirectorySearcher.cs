namespace SolidZip.Core.Contracts.Explorer;

public interface IDirectorySearcher
{
    public ValueTask<string> Search(string path, string pattern);
}