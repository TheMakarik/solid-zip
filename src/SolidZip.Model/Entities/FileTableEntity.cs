namespace SolidZip.Model.Entities;

public record struct FileTableEntity(string FileName, decimal Size, DateTime CreationalTime, DateTime LastChangingTime);