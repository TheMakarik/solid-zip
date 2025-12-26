local button = {}

function button.ctor()
    local button_instance = {};
    local dispatcher = require("szlua.ui.dispatcher");
    dispatcher.exec(function()
        button_instance._wpf_button = Button();
        _debug("Created a new button instance")
    end)
    return setmetatable(button_instance, {__index = button});
end

function button:set_icon(icon)
    self.style = "SzIconButton"
    self.icon = icon
end

function button.from_shared(shared, name)
    local converter = require("szlua.private.converter")
    local result =  converter.dotnet_dict_to_table(shared[name])
    result._wpf_button = shared[name .. "_control"]
    return setmetatable(result, {__index = button});
end

function button:to_shared(shared, name)
    local converter = require("szlua.private.converter")
    shared[name .. "_control"] = self:register()
    shared[name] = converter.table_to_dotnet_dict(self)
end

function button:set_content(label, icon)
    
end

function button:build()
    local redirector = require("szlua.events.event_redirector")
    local dispatcher = require("szlua.ui.dispatcher")
    local resources = require("szlua.ui.resources")


    if type(self.onclick) == "string" then
        redirector.redirect(self._wpf_button, "Click", self.onclick)
    end

    if self.style ~= nil then
        dispatcher.exec(function()
            self._wpf_button.Style = resources[style]
        end)
    end
    

    if self.use_icon_and_text then
        dispatcher.exec(function()
            local stack_panel
            self._wpf_button.Content = stack_panel.register()
        end)
    end

    if type(self.margin) == "number" then
        dispatcher.exec(function()
            self._wpf_button.Margin = self.margin
        end)
    end

    if type(self.padding) == "number" then
        dispatcher.exec(function()
            self._wpf_button.Padding = self.padding
        end)
    end

    if type(self.loaded_event) == "string" then
        redirector.redirect(self._wpf_button, "Loaded", self.loaded_event)
    end
    
    if type(self.mouse_left_button_down_event) == "string" then
        redirector.redirect(self._wpf_button, "MouseLeftButtonDown", self.mouse_left_button_down_event)
    end

    if type(self.mouse_left_button_up_event) == "string" then
        redirector.redirect(self._wpf_button, "MouseLeftButtonUp", self.mouse_left_button_up_event)
    end

    if type(self.mouse_right_button_down_event) == "string" then
        redirector.redirect(self._wpf_button, "MouseRightButtonDown", self.mouse_right_button_down_event)
    end

    if type(self.mouse_right_button_up_event) == "string" then
        redirector.redirect(self._wpf_button, "MouseRightButtonUp", self.mouse_right_button_up_event)
    end

    if type(self.mouse_wheel_event) == "string" then
        redirector.redirect(self._wpf_button, "MouseWheel", self.mouse_wheel_event)
    end

    if type(self.tooltip) == string then
        dispatcher.exec(function()
            self._wpf_button.ToolTip = self.tooltip
        end)
    end
end


function button:set_default_style()
    
end

function button:register()
    return button._wpf_button
end

return button