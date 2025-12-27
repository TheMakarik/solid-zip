local button = {}

local ui_element = require("szlua.ui.sz_ui_element")

function button.ctor()
    local button_instance = {}
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        button_instance._wpf_button = Button()
        _debug("Created a new button instance")
    end)
    return setmetatable(button_instance, {__index = button})
end

function button:set_icon(icon)
    self.style = "SzIconButton"
    self.icon = icon
end

function button.from_shared(shared, name)
    local converter = require("szlua.private.converter")
    local result = converter.dotnet_dict_to_table(shared[name])
    result._wpf_button = shared[name .. "_control"]
    return setmetatable(result, {__index = button})
end

function button:to_shared(shared, name)
    local converter = require("szlua.private.converter")
    shared[name .. "_control"] = self:register()
    shared[name] = converter.table_to_dotnet_dict(self)
end

function button:set_content(label, icon)
  
end

function button:build()
    ui_element.register_base(self._wpf_button, self)
    
    if type(self.onclick) == "string" then
        ui_element.register_event(self._wpf_button, "Click", self.onclick)
    end

    
    if self.style ~= nil then
        ui_element.use_style(self._wpf_button, self.style)
    end
    
    if self.use_icon_and_text then
        local dispatcher = require("szlua.ui.dispatcher")
        dispatcher.exec(function()
            local stack_panel
            self._wpf_button.Content = stack_panel.register()
        end)
    end
end

function button:set_default_style()
end

function button:register()
    return self._wpf_button
end

return button