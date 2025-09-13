
namespace SolidZip.Model.Options;

public class AppDataOptions
{
    public required string DataJsonFilePath { get; set; }
    public required AppDataContent Defaults { get; set; }
}