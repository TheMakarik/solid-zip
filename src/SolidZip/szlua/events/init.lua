local events = {}

function events.raise(event_name, args)
    local raiser = _sz("ILuaEventRaiser")
    if args == nil then
        raiser:RaiseBackground(event_name)
    else
        raiser:RaiseBackground(event_name, args)
    end
end

function events.get_scripts(event_name)
    local lua_events = _sz("ILuaEvents")
    local converter = require("szlua\\private\\converter")
    return converter.to_table_from_array(lua_events:Get(event_name))
end

return events