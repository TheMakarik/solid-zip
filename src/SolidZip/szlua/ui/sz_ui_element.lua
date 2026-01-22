local sz_ui_element = {};

local dispatcher = require("szlua.ui.dispatcher")
local redirector = require("szlua.events.event_redirector")

if (_G.import ~= nil) then
    import('System', 'System.Windows')
    import('PresentationFramework', 'System.Windows')
    import('System.Windows.Controls')
    import("System.Windows.Media")
end

function sz_ui_element.register_event(owner, wpf_event, szlua_event)
    assert(type(wpf_event) == "string")
    if type(szlua_event) == "string" then
        redirector.redirect(owner, wpf_event, szlua_event)
    elseif szlua_event ~= nil then
        error("Unexpected type for szlua event " .. type(szlua_event))
    end
end

function sz_ui_element.register_base(owner, control)

    set_no_colors = set_no_colors or false

    sz_ui_element.register_field(owner, "Margin", control.margin, "number")
    sz_ui_element.register_field(owner, "Padding", control.padding, "number")
    sz_ui_element.register_field(owner, "ToolTip", control.tooltip, "string")
    sz_ui_element.register_field(owner, "Width", control.width, "number")
    sz_ui_element.register_field(owner, "Height", control.height, "number")
    sz_ui_element.register_field(owner, "MaxWidth", control.max_width, "number")
    sz_ui_element.register_field(owner, "MaxHeight", control.max_height, "number")
    sz_ui_element.register_field(owner, "MinWidth", control.min_width, "number")
    sz_ui_element.register_field(owner, "MinHeight", control.min_height, "number")

    sz_ui_element.register_event(owner, "Loaded", control.loaded_event)
    sz_ui_element.register_event(owner, "MouseLeftButtonDown", control.mouse_left_button_down_event)
    sz_ui_element.register_event(owner, "MouseLeftButtonUp", control.mouse_left_button_up_event)
    sz_ui_element.register_event(owner, "MouseRightButtonDown", control.mouse_right_button_down_event)
    sz_ui_element.register_event(owner, "MouseRightButtonUp", control.mouse_right_button_up_event)
    sz_ui_element.register_event(owner, "MouseWheel", control.mouse_wheel_event)
    sz_ui_element.register_event(owner, "MouseEnter", control.mouse_enter_event)
    sz_ui_element.register_event(owner, "MouseLeave", control.mouse_leave_event)
    sz_ui_element.register_event(owner, "MouseDoubleClick", control.mouse_double_click_event)

    if type(control.foreground) == "nil" then
        dispatcher.exec(function()
            owner:SetResourceReference(Control.ForegroundProperty, "ForegroundColorBrush")
        end)
    elseif type(control.foreground) == "string" then
        dispatcher.exec(function()
            owner.Foreground = BrushConverter():ConvertFrom(control.foreground)
        end)
    elseif type(control.foreground) == "userdata" then
        dispatcher.exec(function()
            owner.Foreground = control.foreground
        end)
    end

    if type(control.background) == "nil" then
        dispatcher.exec(function()
            owner:SetResourceReference(Control.BackgroundProperty, "BackgroundColorBrush")
        end)
    elseif type(control.background) == "string" then
        dispatcher.exec(function()
            owner.Background = BrushConverter():ConvertFrom(control.background)
        end)
    elseif type(control.background) == "userdata" then
        dispatcher.exec(function()
            owner.Background = control.background
        end)
    end

    if type(control.border_brush) == "string" then
        dispatcher.exec(function()
            owner.BorderBrush = BrushConverter():ConvertFrom(control.border_brush)
        end)
    elseif type(control.border_brush) == "userdata" then
        dispatcher.exec(function()
            owner.BorderBrush = control.border_brush
        end)
    end

    if type(control.font_size) == "number" then
        dispatcher.exec(function()
            owner.FontSize = control.font_size
        end)
    else
        dispatcher.exec(function()
            if owner.FontSize ~= nil then
                owner.FontSize = 11
            end
        end)
    end

    if type(control.horizontal_alignment) == "string" then
        control.horizontal_alignment = control.horizontal_alignment:lower()
        if control.horizontal_alignment == "stretch" then
            dispatcher.exec(function()
                owner.HorizontalAlignment = HorizontalAlignment.Stretch
            end)
        end
        if control.horizontal_alignment == "center" then
            dispatcher.exec(function()
                owner.HorizontalAlignment = HorizontalAlignment.Center
            end)
        end
        if control.horizontal_alignment == "left" then
            dispatcher.exec(function()
                control.HorizontalAlignment = HorizontalAlignment.Left
            end)
        end
        if control.horizontal_alignment == "right" then
            dispatcher.exec(function()
                owner.HorizontalAlignment = HorizontalAlignment.Right
            end)
        end
    end

    if type(control.horizontal_content_alignment) == "string" then
        control.horizontal_content_alignment = control.horizontal_content_alignment:lower()
        if control.horizontal_content_alignment == "stretch" then
            dispatcher.exec(function()
                owner.HorizontalContentAlignment = HorizontalAlignment.Stretch
            end)
        end
        if control.horizontal_content_alignment == "center" then
            dispatcher.exec(function()
                owner.HorizontalContentAlignment = HorizontalAlignment.Center
            end)
        end
        if control.horizontal_content_alignment == "left" then
            dispatcher.exec(function()
                owner.HorizontalContentAlignment = HorizontalAlignment.Left
            end)
        end
        if control.horizontal_content_alignment == "right" then
            dispatcher.exec(function()
                owner.HorizontalContentAlignment = HorizontalAlignment.Right
            end)
        end
    end

    if type(control.vertical_alignment) == "string" then
        control.vertical_alignment = control.vertical_alignment:lower()
        if control.vertical_alignment == "stretch" then
            dispatcher.exec(function()
                owner.VerticalAlignment = VerticalAlignment.Stretch
            end)
        end
        if control.vertical_alignment == "center" then
            dispatcher.exec(function()
                owner.VerticalAlignment = VerticalAlignment.Center
            end)
        end
        if control.vertical_alignment == "top" then
            dispatcher.exec(function()
                owner.VerticalAlignment = VerticalAlignment.Top
            end)
        end
        if control.vertical_alignment == "botton" then
            dispatcher.exec(function()
                owner.VerticalAlignment = VerticalAlignment.Bottom
            end)
        end
    end

    if type(control.vertical_content_alignment) == "string" then
        control.vertical_content_alignment = control.vertical_content_alignment:lower()
        if control.vertical_content_alignment == "stretch" then
            dispatcher.exec(function()
                control.VerticalContentAlignment = VerticalAlignment.Stretch
            end)
        end
        if control.vertical_content_alignment == "center" then
            dispatcher.exec(function()
                control.VerticalContentAlignment = VerticalAlignment.Center
            end)
        end
        if control.vertical_content_alignment == "top" then
            dispatcher.exec(function()
                control.VerticalContentAlignment = VerticalAlignment.Top
            end)
        end
        if control.vertical_content_alignment == "botton" then
            dispatcher.exec(function()
                control.VerticalContentAlignment = VerticalAlignment.Bottom
            end)
        end
    end

end

function sz_ui_element.register_field(owner, wpf_property, szlua_field, lua_type)
    if type(szlua_field) ~= lua_type then
        return
    end
    assert(type(wpf_property) == "string")
    dispatcher.exec(function()
        getmetatable(owner).__newindex(owner, wpf_property, szlua_field)
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