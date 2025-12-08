local event_redirector = {};

---Redirect .NET event to szlua event
---@param owner any .NET class with event
---@param dotnet_event string dotnet event name
---@param event_name string szlua event name
---@param args table args for szlua event
function event_redirector.redirect(owner, dotnet_event, event_name, args)
    redirect_to(owner, dotnet_event, event_name, args);
end

return event_redirector;