local dialog = {}

local ui_element = require("szlua.ui.sz_ui_element")

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

function dialog.ctor()
    local dialog_instance = {}
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        dialog_instance._wpf_window = Window()
        _debug("Created a new dialog instance")
    end)
    return setmetatable(dialog_instance, {__index = dialog})
end

function dialog.from_shared(shared, name)
    local converter = require("szlua.private.converter")
    local result = converter.dotnet_dict_to_table(shared[name])
    result._wpf_window = shared[name .. "_control"]
    return setmetatable(result, {__index = dialog})
end

function dialog:to_shared(shared, name)
    local converter = require("szlua.private.converter")
    shared[name .. "_control"] = self._wpf_window
    shared[name] = converter.table_to_dotnet_dict(self)
end

function dialog:build()
    local dispatcher = require("szlua.ui.dispatcher")

    if self._off_default_style or false then
        dispatcher.exec(function()
            self._wpf_window.WindowStyle = WindowStyle.None
        end)
    end

    ui_element.register_base(self._wpf_window, self)

    ui_element.register_event(self._wpf_window, "Closed", self.closed_event)
    ui_element.register_event(self._wpf_window, "LocationChanged", self.location_changed_event)
    ui_element.register_event(self._wpf_window, "SizeChanged", self.size_changed_event)

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
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_window:ShowDialog()
    end)
end

function dialog:close()
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_window:Close()
    end)
end

function dialog:minimize()
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_window.WindowsState = WindowState.Minimized
    end)
end

function dialog:normalize()
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        if self._wpf_window.WindowsState == WindowState.Maximized then
            self._wpf_window.WindowsState = WindowStateNormal
        else
            self._wpf_window.WindowsState = WindowState.Maximized
        end
    end)
end

function dialog:drag_move()
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_window:DragMove()
    end)
end

function dialog:set_content(content)
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_window.Content = content:register()
    end)
end

function dialog:off_default_style()
    self._off_default_style = true
end

return dialog