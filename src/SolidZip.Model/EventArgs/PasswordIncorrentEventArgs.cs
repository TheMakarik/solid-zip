namespace SolidZip.Model.EventArgs;

public class PasswordIncorrectEventArgs(string archivePath) : System.EventArgs
{
    public string ArchivePath { get; } = archivePath;
    public bool Retry { get; set; }
}