using System.Runtime.InteropServices;
using SolidZip.Core.Contracts.Win32API;

namespace SolidZip.Modules.Win32API;

public class ConsoleAttacher : IConsoleAttacher, IDisposable
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    private static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleTextAttribute(nint hConsoleOutput, ushort wAttributes);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool WriteConsoleW(nint hConsoleOutput, string lpBuffer, uint nNumberOfCharsToWrite, 
        out uint lpNumberOfCharsWritten, nint lpReserved);

    private const ConsoleColor DefaultForegroundColor = ConsoleColor.White;
    private const int STD_OUTPUT_HANDLE = -11;
    private nint _consoleHandle;
    private bool _isAttached = false;
    
    public void Attach()
    {
        if (_isAttached) 
            return;
        
        AllocConsole();
        _consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        _isAttached = true;
    }

    public void Print(string text, ConsoleColor color)
    {
        if (!_isAttached)
           return;
        
        SetConsoleTextAttribute(_consoleHandle, (ushort)color);
      
        WriteConsoleW(_consoleHandle, text, (uint)text.Length, out _, 0);
      
        SetConsoleTextAttribute(_consoleHandle, (ushort)DefaultForegroundColor);
    }

    public void Dispose()
    {
        if (!_isAttached) 
            return;
        
        FreeConsole();
        _isAttached = false;
    }
}