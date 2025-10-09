application = {};

---Current WPF Application;
application.current = fromSz("Application")

---@param resName string WPF Resource name
---Getting resource from WPF Application
function application.getRes(resName)
    return application.Resources[resName];
end

---Shutdown WPF Application
function application.shutdown()
    application:Shutdown();
end 

return application;