namespace SolidZip.Core.SzLuaRecords;

public record LuaLoadMenuItem(
    //DO NOT RENAME THIS PROPERTY, IT NEED TO BE CALLED TO DUE TO LUA CONVERSIONS
    // ReSharper disable once InconsistentNaming
    ItemsControl items_control,
    // ReSharper disable once InconsistentNaming
    object? args
);