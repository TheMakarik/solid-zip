local loc = {}

if (_G.import ~= nil) then
    import("System.Globalization");
end

function loc.change_silently(newLoc)
    assert(type(newLoc) == 'string', "localization must be string");
    local user_json_manager = _sz("IUserJsonManager");
    user_json_manager:ChangeCurrentCulture(CultureInfo.GetCultureInfo(newLoc));
end

function loc.current()
    return CultureInfo.CurrentUICulture:ToString();
end

return loc;
