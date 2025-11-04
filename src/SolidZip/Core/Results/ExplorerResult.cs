namespace SolidZip.Core.Results;

public enum ExplorerResult : byte
{
    Success,
    UnauthorizedAccess,
    NotDirectory,
    UnexistingDirectory
}