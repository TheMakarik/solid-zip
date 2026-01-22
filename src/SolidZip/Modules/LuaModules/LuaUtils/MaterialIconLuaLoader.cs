using Material.Icons.WPF;

namespace SolidZip.Modules.LuaModules.LuaUtils;

public class MaterialIconLuaLoader(ILuaDebugConsole console)
{
    public virtual MaterialIcon Load(string kind, string scriptPath)
    {
        var couldParse = Enum.TryParse<MaterialIconKind>(kind, out var result);
        if (couldParse)
            return new MaterialIcon { Kind = result };

        console.PrintAsync($"ERROR: Cannot load icon {kind} from material icons pack", scriptPath, ConsoleColor.Red);
        return new MaterialIcon { Kind = MaterialIconKind.Error };
    }
}