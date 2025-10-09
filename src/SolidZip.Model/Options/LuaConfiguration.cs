

namespace SolidZip.Model.Options;

public class LuaConfiguration
{
    public required Encoding Encoding { get; set; }
    public required string[] ScriptsFolders { get; set; }
    public required string[] Modules { get; set; }
    public required string LuaExtensionsPattern { get; set; }
    public required string ExecuteFunctionName { get; set; }
    public required string EventsField { get; set; }
}