local dispatcher = {};

function dispatcher.exec(func)
    Application.Current.Dispatcher:Invoke(func);
end
return dispatcher;