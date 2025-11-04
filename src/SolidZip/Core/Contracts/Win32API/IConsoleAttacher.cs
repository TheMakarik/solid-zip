namespace SolidZip.Core.Contracts.Win32API;

public interface IConsoleAttacher
{
    public void Attach();
    public void Print(string text, ConsoleColor color);
}