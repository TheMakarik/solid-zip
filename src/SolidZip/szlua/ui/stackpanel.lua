local stack_panel = {};

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end


function stack_panel.ctor()
    local stack_panel_instance = {};
    local dispatcher = require("szlua.ui.dispatcher");
    dispatcher.exec(function()
        stack_panel_instance._wpf_stak_panel = StackPanel();
        _debug("Created a new stack_panel instance")
    end)
    return setmetatable(stack_panel_instance, {__index = stack_panel});
end

function stack_panel.from_shared(shared, name)
    local converter = require("szlua.private.converter")
    local result =  converter.dotnet_dict_to_table(shared[name])
    result._wpf_stack_panel = shared[name .. "_control"]
    return setmetatable(result, {__index = stack_panel});
end

function stack_panel:to_shared(shared, name)
    local converter = require("szlua.private.converter")
    shared[name .. "_control"] = self._wpf_stack_panel
    shared[name] = converter.table_to_dotnet_dict(self)
end

function stack_panel:build()
    local dispatcher = require("szlua.ui.dispatcher")
    if self.horizontal or false then
        dispatcher.exec(function()
            self._wpf_stack_panel.Orientation = Orientation.Horizontal
        end)
    else
        dispatcher.exec(function()
            self._wpf_stack_panel.Orientation = Orientation.Vertical
        end)
    end

    if type(self.margin) == "number" then
        dispatcher.exec(function()
            self._wpf_stack_panel.Margin = self.margin
        end)
    end

    if type(self.padding) == "number" then
        dispatcher.exec(function()
            self._wpf_stack_panel.Padding = self.padding
        end)
    end

    if type(self.loaded_event) == "string" then
        redirector.redirect(self._wpf_stack_panel, "Loaded", self.loaded_event)
    end
    

    if type(self.location_changed_event) == "string" then
        redirector.redirect(self._wpf_stack_panel, "LocationChanged", self.location_changed_event)
    end

    if type(self.size_changed_event) == "string" then
        redirector.redirect(self._wpf_stack_panel, "SizeChanged", self.size_changed_event)
    end


    if type(self.mouse_left_button_down_event) == "string" then
        redirector.redirect(self._wpf_stack_panel, "MouseLeftButtonDown", self.mouse_left_button_down_event)
    end

    if type(self.mouse_left_button_up_event) == "string" then
        redirector.redirect(self._wpf_stack_panel, "MouseLeftButtonUp", self.mouse_left_button_up_event)
    end

    if type(self.mouse_right_button_down_event) == "string" then
        redirector.redirect(self._wpf_stack_panel, "MouseRightButtonDown", self.mouse_right_button_down_event)
    end

    if type(self.mouse_right_button_up_event) == "string" then
        redirector.redirect(self._wpf_stack_panel, "MouseRightButtonUp", self.mouse_right_button_up_event)
    end


    if type(self.mouse_wheel_event) == "string" then
        redirector.redirect(self._wpf_stack_panel, "MouseWheel", self.mouse_wheel_event)
    end
    
end

function stack_panel:add(content)
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function() 
        self._wpf_stack_panel.Children:Add(content:register())
    end)
end

function stack_panel:set_orientation(orientation)
    orientation = orientation:lower()
    if orientation == "horizontal" then
        self.horizontal = true
    elseif orientation == "vertical" then
        self.horizontal = false
    else 
        error("Cannot set orientation of type: " .. orientation .. "available only horizontal or \"vertical")
    end
end

function stack_panel:register()
    return self._wpf_stack_panel
end


return stack_panel