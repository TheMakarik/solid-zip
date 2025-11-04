namespace SolidZip.Core.Contracts.Win32API;

public interface IAssociatedIconExtractor : IDisposable
{
    public nint Extract(string path);
}