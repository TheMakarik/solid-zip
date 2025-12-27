local sz_ui_element = {};

local dispatcher = require("szlua.ui.dispatcher")

function sz_ui_element.register_event(owner, wpf_event, szlua_event)
    assert(type(wpf_event) == "string")
    if type(szlua_event) == "string" then
        local redirector = require("szlua.events.event_redirector")
        redirector.redirect(owner, wpf_event, szlua_event)
    elseif szlua_event ~= nil then
        error("Unexpected type for szlua event " .. type(szlua_event))
    end
end

function sz_ui_element.register_base(owner, control)
    local redirector = require("szlua.events.event_redirector")
    if type(control.margin) == "number" then
        dispatcher.exec(function()
            owner.Margin = control.margin
        end)
    end

    if type(control.padding) == "number" then
        dispatcher.exec(function()
            owner.Padding = control.padding
        end)
    end

    if type(control.loaded_event) == "string" then
        redirector.redirect(owner, "Loaded", control.loaded_event)
    end

    if type(control.mouse_left_button_down_event) == "string" then
        redirector.redirect(owner, "MouseLeftButtonDown", control.mouse_left_button_down_event)
    end

    if type(control.mouse_left_button_up_event) == "string" then
        redirector.redirect(owner, "MouseLeftButtonUp", control.mouse_left_button_up_event)
    end

    if type(control.mouse_right_button_down_event) == "string" then
        redirector.redirect(owner, "MouseRightButtonDown", control.mouse_right_button_down_event)
    end

    if type(control.mouse_right_button_up_event) == "string" then
        redirector.redirect(owner, "MouseRightButtonUp", control.mouse_right_button_up_event)
    end

    if type(control.mouse_wheel_event) == "string" then
        redirector.redirect(owner, "MouseWheel", control.mouse_wheel_event)
    end

    if type(control.tooltip) == string then
        dispatcher.exec(function()
            owner.ToolTip = control.tooltip
        end)
    end
end

function sz_ui_element.register_field(owner, wpf_property, szlua_field, lua_type)
    if type(szlua_field) ~= lua_type then
        return
    end
    assert(type(wpf_property) == "string")
    dispatcher.exec(function()
        owner[wpf_property] = szlua_field
    end)
end

function sz_ui_element.use_style(owner, style)
    local res = require("szlua.ui.resources")
    assert(type(style) == "string")
    dispatcher.exec(function()
        owner.Style = res[style]
        _debug("Now " .. owner:ToString() .. " using style " .. style)
    end)
end

return sz_ui_element