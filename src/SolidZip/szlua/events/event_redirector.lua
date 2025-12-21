local event_redirector = {};

function event_redirector.redirect(owner, dotnet_event, event_name)
    redirect_to(owner, dotnet_event, event_name);
end

return event_redirector;