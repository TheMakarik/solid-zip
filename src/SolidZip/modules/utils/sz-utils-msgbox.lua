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

---Show a question message box
---@param message string The question text
---@param title string|nil The title of the message box
---@param buttons number|nil The button configuration to use (default: Yes/No)
---@param icon number|nil The icon to display (default: Question)
---@return number The dialog result
function msgbox.showQuestion(message, title, buttons, icon)
    local actualTitle = title or "Question"
    local actualButtons = buttons or msgbox.buttons.yesNo
    local actualIcon = icon or msgbox.icons.question

    return msgbox.show(message, actualTitle, actualButtons, actualIcon)
end

---Show a warning message box
---@param message string The warning text
---@param title string|nil The title of the message box
---@param buttons number|nil The button configuration to use (default: OK)
---@param icon number|nil The icon to display (default: Warning)
---@return number The dialog result
function msgbox.showWarn(message, title, buttons, icon)
    local actualTitle = title or "Warning"
    local actualButtons = buttons or msgbox.buttons.ok
    local actualIcon = icon or msgbox.icons.warning

    return msgbox.show(message, actualTitle, actualButtons, actualIcon)
end

---Show an information message box
---@param message string The information text
---@param title string|nil The title of the message box
---@param buttons number|nil The button configuration to use (default: OK)
---@param icon number|nil The icon to display (default: Information)
---@return number The dialog result
function msgbox.showInfo(message, title, buttons, icon)
    local actualTitle = title or "Information"
    local actualButtons = buttons or msgbox.buttons.ok
    local actualIcon = icon or msgbox.icons.information

    return msgbox.show(message, actualTitle, actualButtons, actualIcon)
end

---Show an error message box
---@param message string The error text
---@param title string|nil The title of the message box
---@param buttons number|nil The button configuration to use (default: OK)
---@param icon number|nil The icon to display (default: Error)
---@return number The dialog result
function msgbox.showError(message, title, buttons, icon)
    local actualTitle = title or "Error"
    local actualButtons = buttons or msgbox.buttons.ok
    local actualIcon = icon or msgbox.icons.error

    return msgbox.show(message, actualTitle, actualButtons, actualIcon)
end

---Show a confirmation message box
---@param message string The confirmation text
---@param title string|nil The title of the message box
---@param buttons number|nil The button configuration to use (default: Yes/No)
---@param icon number|nil The icon to display (default: Question)
---@return number The dialog result
function msgbox.showConfirm(message, title, buttons, icon)
    local actualTitle = title or "Confirmation"
    local actualButtons = buttons or msgbox.buttons.yesNo
    local actualIcon = icon or msgbox.icons.question

    return msgbox.show(message, actualTitle, actualButtons, actualIcon)
end

---Show a critical error message box
---@param message string The critical error text
---@param title string|nil The title of the message box
---@param buttons number|nil The button configuration to use (default: OK)
---@param icon number|nil The icon to display (default: Stop)
---@return number The dialog result
function msgbox.showCritical(message, title, buttons, icon)
    local actualTitle = title or "Critical Error"
    local actualButtons = buttons or msgbox.buttons.ok
    local actualIcon = icon or msgbox.icons.stop

    return msgbox.show(message, actualTitle, actualButtons, actualIcon)
end

---Show an exclamation message box
---@param message string The message text
---@param title string|nil The title of the message box
---@param buttons number|nil The button configuration to use (default: OK)
---@param icon number|nil The icon to display (default: Exclamation)
---@return number The dialog result
function msgbox.showExclamation(message, title, buttons, icon)
    local actualTitle = title or "Attention"
    local actualButtons = buttons or msgbox.buttons.ok
    local actualIcon = icon or msgbox.icons.exclamation

    return msgbox.show(message, actualTitle, actualButtons, actualIcon)
end

return msgbox;