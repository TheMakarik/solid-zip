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

function button:set_icon_as_content(icon)
    self.style = "SzIconButton"
    self.icon = icon
end

function button:set_content()

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

    if self.use_icon_and_text or false then
        
    end
end
    
    
end

function button:set_default_style()
    
end

function button:register()
    return button._wpf_button
end

return button