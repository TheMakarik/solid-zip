namespace SolidZip.Domain.Entities;

public record FileEntity(
    string Path,
    bool IsDirectory,
    bool IsArchiveEntry,
    DateTime CreationalTime, 
    DateTime LastChangingTime);