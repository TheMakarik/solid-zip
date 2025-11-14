
local events = {};

events._eventRaiser = _G.fromSz('ILuaExtensionsRaiser');

---@param eventName string name of the event to raise
---Raises custom event for extensions
function events.raise(eventName)
    assert(events._eventRaiser ~= nil, "event raiser must be initialized")
end
    
function events.raiseWithResult(eventName)
    assert(events._eventRaiser ~= nil, "event raiser must be initialized")
end

return events;