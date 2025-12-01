local localstr = {}
localstr.__index = localstr

---Create a new localization string, the table for automatic selecting user language and return localized string for it
---@return table new localization string 
function localstr.ctor()
    local self = setmetatable({}, LocalStr)
    return self
end

---Set a value for specific localization
---@param loc string localization for example ru-RU, "" for default localization
---@param value string string what will be return at this localization for example on ru-RU -> "Привет, Мир!"
function localstr:on(loc, value)
    self[loc] = value
end

---Set a default value, when user localization do not added to localization string
function localstr:default(value)
    self[""] = value
end

---Gets the user localization and based on loaded localization returns string
---@return string configured on this localization string or default
function localstr:build()
    local loc = require("szlua\\loc")
    local current_loc = loc.current();
    if self[current_loc] ~= nil then
        return self[current_loc]
    end
    return self[""]
end

return localstr;