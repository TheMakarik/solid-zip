local loc = {}

if(_G.import ~= nil) then
    import("System");
end

---change application current culture to the new
---@param newLoc string culture 
function loc.change(newLoc)
    assert(type(newLoc) == 'string', "localization must be string");
    
end

---Gets current application culture
---@return string current culture
function loc.current()
    return CultureInfo.CurrentUICulture:ToString();
end

return loc;
