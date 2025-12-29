local button = {}

local ui_element = require("szlua.ui.sz_ui_element")

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

function button.ctor()
    local button_instance = {}
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        button_instance._wpf_button = Button()
    end)
    return setmetatable(button_instance, {__index = button})
end

function button:set_icon(icon)
    ui_element.use_style(self._wpf_button, "SzIconButton")
    self.icon = icon

    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_button.Content = icon
    end)
  
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
    if icon ~= nil then
        self.use_icon_and_label = true
        local dispatcher = require("szlua.ui.dispatcher")
        dispatcher.exec(function()
            local stack_panel = require("szlua.ui.stackpanel")
            local icon_and_text = stack_panel.ctor()
            icon_and_text:set_orientation("horizontal")
            icon_and_text:add(icon:register())
            if type(label) == "string" then
                local text = label
                label = require("szlua.ui.label").ctor()
                label.content = text
            end
            icon_and_text:add(label:register() or "")
            self._wpf_button.Content = icon_and_text:register()
          
        end)

    else
        local dispatcher = require("szlua.ui.dispatcher")
       
        dispatcher.exec(function()
            if type(label) == "string" then
                local text = label
                local label_builder = require("szlua.ui.label")
                label = label_builder.ctor()
                _debug_print(text)
                label.content = text
            end
            self._wpf_button.Content = label:build():register()
        end)
    end
    
end

function button:build()
    
    ui_element.register_base(self._wpf_button, self)
    
    if type(self.onclick_event) == "string" then
        ui_element.register_event(self._wpf_button, "Click", self.onclick_event)
    end
    
    
    return self
end


function button:register()
    return self._wpf_button
end

return button