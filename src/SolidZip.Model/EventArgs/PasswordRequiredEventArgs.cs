namespace SolidZip.Model.EventArgs;

public class PasswordRequiredEventArgs(string archivePath) : System.EventArgs
{
    public string ArchivePath { get; init; } = archivePath;
    public bool Cancel { get; set; }
    public string Password { get; set; } = string.Empty;
}