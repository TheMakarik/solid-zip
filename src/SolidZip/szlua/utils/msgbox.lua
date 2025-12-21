local msgbox = {};

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
end


msgbox._wpfMessageBox = _G.MessageBox;

msgbox.icons = {
    ---No icon
    none = MessageBoxImage.None,
    ---Error icon
    error = MessageBoxImage.Error,
    ---Warning icon  
    warning = MessageBoxImage.Warning,
    ---Information icon
    information = MessageBoxImage.Information,
    ---Question icon
    question = MessageBoxImage.Question,
    ---Stop icon
    stop = MessageBoxImage.Stop,
    ---Exclamation icon
    exclamation = MessageBoxImage.Exclamation
}


msgbox.buttons = {
    ok = MessageBoxButton.OK,
    okCancel = MessageBoxButton.OKCancel,
    yesNo = MessageBoxButton.YesNo,
    yesNoCancel = MessageBoxButton.YesNoCancel
}

msgbox.results = {
    none = MessageBoxResult.None,
    ok = MessageBoxResult.OK,
    cancel = MessageBoxResult.Cancel,
    yes = MessageBoxResult.Yes,
    no = MessageBoxResult.No
}

msgbox.defaults = {
    message = "",
    title = "Message",
    buttons = MessageBoxButton.OK,
    icon = MessageBoxImage.None
}

function msgbox.setDefaults(message, title, buttons, icon)
    msgbox.defaults.message = message or msgbox.defaults.message
    msgbox.defaults.title = title or msgbox.defaults.title
    msgbox.defaults.buttons = buttons or msgbox.defaults.buttons
    msgbox.defaults.icon = icon or msgbox.defaults.icon
end


function msgbox.show(message, title, buttons, icon)
    local actualMessage = message or msgbox.defaults.message
    local actualTitle = title or msgbox.defaults.title
    local actualButtons = buttons or msgbox.defaults.buttons
    local actualIcon = icon or msgbox.defaults.icon

    return msgbox._wpfMessageBox.Show(actualMessage, actualTitle, actualButtons, actualIcon)
end

return msgbox;