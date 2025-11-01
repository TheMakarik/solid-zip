
using SolidZip.Deprecated.Model.Entities;

namespace SolidZip.Deprecated.Model.Options;

public class AppDataOptions
{
    public required string DataJsonFilePath { get; set; }
    public required AppDataContent Defaults { get; set; }
}