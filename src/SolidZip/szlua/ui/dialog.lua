local dialog = {};

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

---Creates a dialog instance
function dialog.ctor()
    local dialog_instance = {};
    local dispatcher = require("szlua.ui.dispatcher");
    dispatcher.exec(function()
        dialog_instance._wpf_window = Window();
    end)
    return setmetatable(dialog_instance, dialog);
end

---Builds a dialog instance as WPF Window
function dialog:build()
    self._is_created = true;
end

---Show a dialog instance
function dialog:show()
    if not self._is_created then
        error("Cannot show uncreated dialog, use dialog:build() before invoking dialog:show()")
    end
    local dispatcher = require("szlua.ui.dispatcher");
    dispatcher.exec(function()
        dialog._wpf_window:ShowDialog()
    end)
end

function dialog.maximize()
    
end

function dialog.close()
    
end

function dialog.minimize()
    
end

function dialog.normalize()
    
end

function dialog.set_default_background()
    
end

return dialog;