namespace SolidZip.Core.SzLuaRecords;


public record LuaLoadExplorerElementsView(
    //DO NOT RENAME THIS PROPERTY, IT NEED TO BE CALLED TO DUE TO LUA CONVERSIONS
    // ReSharper disable InconsistentNaming
    string unlocalized,
    string localized
    // ReSharper restore InconsistentNaming
);