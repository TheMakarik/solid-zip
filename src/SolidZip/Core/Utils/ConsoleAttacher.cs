
namespace SolidZip.Core.Utils;

public class ConsoleAttacher : IDisposable
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FlushConsoleInputBuffer(IntPtr hConsoleInput);
    
    
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        nint lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        nint hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetStdHandle(int nStdHandle, nint hHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleTextAttribute(nint hConsoleOutput, ushort wAttributes);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool WriteConsoleW(nint hConsoleOutput, string lpBuffer, 
        uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten, nint lpReserved);
    

    private const uint GenericWrite = 0x40000000;
    private const uint FileShareWrite = 0x2;
    private const uint OpenExisting = 0x3;
    private const int StdOutputHandle = -11;

    private nint _consoleHandle;
    private bool _isAttached = false;

    public virtual void Attach()
    {
        if (_isAttached) 
            return;

        AllocConsole();
        
        var stdHandle = CreateFile("CONOUT$", GenericWrite, FileShareWrite, 0, OpenExisting, 0, 0);
        SetStdHandle(StdOutputHandle, stdHandle);

        _consoleHandle = GetStdHandle(StdOutputHandle);
        
        _isAttached = true;
    }

    public virtual void Print(string text, ConsoleColor color)
    {
        if (!_isAttached)
            return;
        
        SetConsoleTextAttribute(_consoleHandle, (ushort)color);
        WriteConsoleW(_consoleHandle, text, (uint)text.Length, out _, 0);
        SetConsoleTextAttribute(_consoleHandle, (ushort)ConsoleColor.White);

        FlushConsoleInputBuffer(_consoleHandle);
    }

    public virtual void Dispose()
    {
        if (!_isAttached)
            return;
        
        FreeConsole();
        _isAttached = false;
    }
}