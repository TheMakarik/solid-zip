local dispatcher = {};

---Invokes the function at the ui thread, nice for creating WPF controls or changing it
function dispatcher.exec(func)
    Application.Current.Dispatcher:Invoke(func);
end
return dispatcher;