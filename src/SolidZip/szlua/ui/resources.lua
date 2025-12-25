local res = {}
local mt = {}

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end


mt.__index = function(self, key)
    _debug("Getting resource: " .. key)
    return  Application.Current.Resources:get_Item(key);
end

mt.__newindex = function(self, key, value)
    _debug("Setting resource: " .. key .. "value: " .. value)
    return  Application.Current.Resources:set_Item(key, value);
end

setmetatable(res, mt);

return res