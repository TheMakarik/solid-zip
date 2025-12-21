local dispatcher = {};

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end


function dispatcher.exec(func)
    Application.Current.Dispatcher:Invoke(func);
end
return dispatcher;