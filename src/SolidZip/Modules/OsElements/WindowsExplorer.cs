
namespace SolidZip.Modules.OsElements;

public class WindowsExplorer(ILogger<WindowsExplorer> logger) : IWindowsExplorer
{
    public void Open(string path)
    {
        using var process = new Process();
      
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = $"/C \"explorer {path}\""; 
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true; 
        
        logger.LogInformation($"Opening {path}");
        process.Start();
        process.WaitForExit();
        
    }
}