local label = {}

function label.ctor()
    
end

function label:build()
    local dispatcher = require("szlua.ui.dispatcher")
    
    if type(self.content) == "string" then
        dispatcher.exec(function()
            self._wpf_label.Content = self.content
        end)
    end
    
    if type(self.margin) == "number" then
        dispatcher.exec(function()
            self._wpf_label.Margin = self.margin
        end)
    end

    if type(self.padding) == "number" then
        dispatcher.exec(function()
            self._wpf_label.Padding = self.padding
        end)
    end

    if type(self.loaded_event) == "string" then
        redirector.redirect(self._wpf_label, "Loaded", self.loaded_event)
    end
    

    if type(self.mouse_left_button_down_event) == "string" then
        redirector.redirect(self._wpf_label, "MouseLeftButtonDown", self.mouse_left_button_down_event)
    end

    if type(self.mouse_left_button_up_event) == "string" then
        redirector.redirect(self._wpf_label, "MouseLeftButtonUp", self.mouse_left_button_up_event)
    end

    if type(self.mouse_right_button_down_event) == "string" then
        redirector.redirect(self._wpf_label, "MouseRightButtonDown", self.mouse_right_button_down_event)
    end

    if type(self.mouse_right_button_up_event) == "string" then
        redirector.redirect(self._wpf_label, "MouseRightButtonUp", self.mouse_right_button_up_event)
    end

    if type(self.mouse_wheel_event) == "string" then
        redirector.redirect(self._wpf_label, "MouseWheel", self.mouse_wheel_event)
    end
end

function label:register()
    
end

function label.from_shared(shared, name)
    local converter = require("szlua.private.converter")
    local result =  converter.dotnet_dict_to_table(shared[name])
    result._wpf_label = shared[name .. "_control"]
    return setmetatable(result, {__index = label});
end

function label:to_shared(shared, name)
    local converter = require("szlua.private.converter")
    shared[name .. "_control"] = self:register()
    shared[name] = converter.table_to_dotnet_dict(self)
end

return label