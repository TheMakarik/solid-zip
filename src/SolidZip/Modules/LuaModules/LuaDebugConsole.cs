using SolidZip.Core.Contracts.Win32API;

namespace SolidZip.Modules.LuaModules;

public class LuaDebugConsole(IConsoleAttacher console, ILogger<LuaDebugConsole> logger, IUserJsonManager userJson) : ILuaDebugConsole
{
    public async ValueTask AttachAsync()
    {
        if (!await userJson.GetAttachPluginsConsoleAsync())
            return;
        
        console.Attach();
        console.Print("Solid Zip - Lua Plugins Console", ConsoleColor.White);
    }

    public Task PrintAsync(string text, string scriptPath)
    {
        return Task.Run(() =>
        {
            console.Print($"{scriptPath}:", ConsoleColor.DarkGreen);
            console.Print(text + "\n", ConsoleColor.White);
        });
    }
}