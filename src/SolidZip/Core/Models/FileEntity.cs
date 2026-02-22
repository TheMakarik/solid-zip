namespace SolidZip.Core.Models;

public record struct FileEntity(
    string Path,
    bool IsDirectory,
    DateTime LastChanging,
    DateTime CreationalTime,
    ulong? BytesSize,
    string? Comment = null,
    bool IsArchiveEntry = false,
    bool IsHidden = false
);