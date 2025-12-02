local res = {}
local mt = {}

mt.__index = function(key)
    return  Application.Current.Resources[key];
end
setmetatable(res, mt);

return res