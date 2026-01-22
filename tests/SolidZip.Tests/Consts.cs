using System.IO;
using System.Reflection;

namespace SolidZip.Tests;

public static class Consts
{
    public const string LuaScriptFolder = "SzLuaTests/Scripts/";

    public static readonly string ModulesFolder = Path.GetDirectoryName
            (Assembly.GetExecutingAssembly()?.Location ?? string.Empty)!
        .Replace("tests", "src")
        .Replace(".Tests", string.Empty);
}