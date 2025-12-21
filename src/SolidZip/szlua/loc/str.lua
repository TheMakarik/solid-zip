local localstr = {}
localstr.__index = localstr

function localstr.ctor()
    local self = setmetatable({}, localstr)
    return self
end

function localstr:on(loc, value)
    self[loc] = value
end

function localstr:default(value)
    self[""] = value
end

function localstr:build()
    local loc = require("szlua\\loc")
    local current_loc = loc.current();
    if self[current_loc] ~= nil then
        return self[current_loc]
    end
    return self[""]
end

return localstr;