local msgbox = {};

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
end


msgbox._wpfMessageBox = _G.MessageBox;

---Available icons for message boxes
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

---Available button configurations
msgbox.buttons = {
    ---OK button
    ok = MessageBoxButton.OK,
    ---OK and Cancel buttons
    okCancel = MessageBoxButton.OKCancel,
    ---Yes and No buttons
    yesNo = MessageBoxButton.YesNo,
    ---Yes, No and Cancel buttons
    yesNoCancel = MessageBoxButton.YesNoCancel
}

---Dialog results
msgbox.results = {
    ---No result
    none = MessageBoxResult.None,
    ---OK button clicked
    ok = MessageBoxResult.OK,
    ---Cancel button clicked
    cancel = MessageBoxResult.Cancel,
    ---Yes button clicked
    yes = MessageBoxResult.Yes,
    ---No button clicked
    no = MessageBoxResult.No
}

---Default settings
msgbox.defaults = {
    message = "",
    title = "Message",
    buttons = MessageBoxButton.OK,
    icon = MessageBoxImage.None
}

---Set default values for message boxes
---@param message string|nil Default message text
---@param title string|nil Default title
---@param buttons number|nil Default button configuration
---@param icon number|nil Default icon
function msgbox.setDefaults(message, title, buttons, icon)
    msgbox.defaults.message = message or msgbox.defaults.message
    msgbox.defaults.title = title or msgbox.defaults.title
    msgbox.defaults.buttons = buttons or msgbox.defaults.buttons
    msgbox.defaults.icon = icon or msgbox.defaults.icon
end

---Show a message box with custom parameters
---@param message string The message text to display
---@param title string|nil The title of the message box
---@param buttons number|nil The button configuration to use
---@param icon number|nil The icon to display
---@return number The dialog result
function msgbox.show(message, title, buttons, icon)
    local actualMessage = message or msgbox.defaults.message
    local actualTitle = title or msgbox.defaults.title
    local actualButtons = buttons or msgbox.defaults.buttons
    local actualIcon = icon or msgbox.defaults.icon

    return msgbox._wpfMessageBox.Show(actualMessage, actualTitle, actualButtons, actualIcon)
end

return msgbox;