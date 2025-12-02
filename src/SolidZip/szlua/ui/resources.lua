local res = {}
local mt = {}

mt.__index = function(key)
    _debug("Getting resource: " .. key)
    return  Application.Current.Resources:get_Item(key);
end

mt.__newindex = function(key, value)
    _debug("Setting resource: " .. key .. "value: " .. value)
    return  Application.Current.Resources:set_Item(key, value);
end

setmetatable(res, mt);

return res