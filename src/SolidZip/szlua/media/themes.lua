local theme = {}
local mt = {}

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
    import ("PresentationCore", "System.Windows.Media")
end

function mt.__index(self, key)
    local res = require("szlua.ui.resources")
    key = key:lower()
    if key == "primarycolor" then
        return res.PrimaryColorBrush:ToString()
    end
    if key == "foregroundcolor" then
        return res.ForegroundColorBrush:ToString()
    end
    if key == "foregroundhovercolor" then
        return res.ForegroundHoverColorBrush:ToString()
    end
    if key == "warningcolor" then
        return  res.WarningColorBrush:ToString()
    end
    if key == "backgroundcolor" then
        return res.BackgroundColorBrush:ToString()
    end
    error("Cannot find color " .. key)
end

function mt.__newindex(self, key, value)
    key = key:lower()
   
    if key == "primarycolor" then
        res.PrimaryColorBrush =  BrushConverter():ConvertFrom(value)
    end
    if key == "foregroundcolor" then
        res.ForegroundColorBrush = BrushConverter():ConvertFrom(value)
    end
    if key == "foregroundhovercolor" then
        res.ForegroundHoverColorBrush = BrushConverter():ConvertFrom(value)
    end
    if key == "warningcolor" then
        res.WarningColorBrush =  BrushConverter():ConvertFrom(value)
    end
    if key == "backgroundcolor" then
        res.BackgroundColorBrush =  BrushConverter():ConvertFrom(value)
    end
    error("Cannot find color " .. key)
end

setmetatable(theme, mt)

return theme