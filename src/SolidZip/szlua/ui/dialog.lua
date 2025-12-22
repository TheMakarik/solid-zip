local dialog = {};

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

function dialog.ctor()
    local dialog_instance = {};
    local dispatcher = require("szlua.ui.dispatcher");
    dispatcher.exec(function()
        dialog_instance._wpf_window = Window();
        _debug("Created a new dialog instance")
    end)
    return setmetatable(dialog_instance, {__index = dialog});
end

function dialog:build()
    local redirector = require("szlua.events.event_redirector")
    local dispatcher = require("szlua.ui.dispatcher")

    if self._off_default_style or false then
        dispatcher.exec(function()
            self._wpf_window.WindowStyle = WindowStyle.None
        end)
    end

    if type(self.loaded_event) == "string" then
        redirector.redirect(self._wpf_window, "Loaded", self.loaded_event)
    end

    if type(self.closed_event) == "string" then
        redirector.redirect(self._wpf_window, "Closed", self.closed_event)
    end
    

    if type(self.location_changed_event) == "string" then
        redirector.redirect(self._wpf_window, "LocationChanged", self.location_changed_event)
    end

    if type(self.size_changed_event) == "string" then
        redirector.redirect(self._wpf_window, "SizeChanged", self.size_changed_event)
    end
    

    if type(self.mouse_left_button_down_event) == "string" then
        redirector.redirect(self._wpf_window, "MouseLeftButtonDown", self.mouse_left_button_down_event)
    end

    if type(self.mouse_left_button_up_event) == "string" then
        redirector.redirect(self._wpf_window, "MouseLeftButtonUp", self.mouse_left_button_up_event)
    end

    if type(self.mouse_right_button_down_event) == "string" then
        redirector.redirect(self._wpf_window, "MouseRightButtonDown", self.mouse_right_button_down_event)
    end

    if type(self.mouse_right_button_up_event) == "string" then
        redirector.redirect(self._wpf_window, "MouseRightButtonUp", self.mouse_right_button_up_event)
    end
    

    if type(self.mouse_wheel_event) == "string" then
        redirector.redirect(self._wpf_window, "MouseWheel", self.mouse_wheel_event)
    end
    

    if type(self.title) == "string" then
        dispatcher.exec(function()
            self._wpf_window.Title = self.title
        end)
    end

    self._is_created = true
end

function dialog:show()
    if not self._is_created then
        error("Cannot show uncreated dialog, use dialog:build() before invoking dialog:show()")
    end
    local dispatcher = require("szlua.ui.dispatcher");
    dispatcher.exec(function()
        self._wpf_window:ShowDialog()
    end)
end

function dialog.maximize()
    dispatcher.exec(function()
        self._wpf_window.WindowsState = WindowState.Maximized
    end)
end

function dialog.close()
    
end

function dialog.minimize()
    dispatcher.exec(function()
        self._wpf_window.WindowsState = WindowState.Minimized
    end)
end

function dialog.normalize()
    
end

function dialog:drag_move()
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_window:DragMove()
    end)
    
end

function dialog:off_default_style()
    self._off_default_style = true
end

return dialog;