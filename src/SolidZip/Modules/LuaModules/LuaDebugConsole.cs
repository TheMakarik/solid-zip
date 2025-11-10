using SolidZip.Core.Utils;

namespace SolidZip.Modules.LuaModules;

public class LuaDebugConsole(ConsoleAttacher console, ILogger<LuaDebugConsole> logger, IUserJsonManager userJson) : ILuaDebugConsole
{
    public async ValueTask AttachAsync()
    {
        if (!await userJson.GetAttachPluginsConsoleAsync())
            return;
        
        console.Attach();
        Console.Title = "Plugin's output";
        console.Print("Solid Zip - Lua Plugin's Console, use script.debug.print(message) to write something here\n", ConsoleColor.Yellow);
        logger.LogInformation("Lua plugins console was loaded");
    }

    public Task PrintAsync(string text, string scriptPath, ConsoleColor scriptPathColor = ConsoleColor.Green)
    {
        console.Print($"{scriptPath}:", scriptPathColor);
        console.Print(text + "\n", ConsoleColor.White);
        return Task.CompletedTask;
    }
}