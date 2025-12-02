namespace SolidZip.Modules.LuaModules.Utils;

public class MaterialIconLuaLoader(ILuaDebugConsole console)
{
    public virtual MaterialIconKind Load(string kind, string scriptPath)
    {
        var couldParse = Enum.TryParse<MaterialIconKind>(kind, out var result);
        if (!couldParse)
            console.PrintAsync($"ERROR: Cannot load icon {kind} from material icons pack", scriptPath, ConsoleColor.Red);
        return result;
    }
}