script.events = { 'service_menu_item_loaded_ret',
                  'silver_pack1_description_ret',
                  "history_menu_item_onclick",
                  "history_close",
                  "history_close_hover",
                  "history_close_stop_hover",
                  "history_normalize",
                  "history_normalize_hover",
                  "history_normalize_stop_hover",
                  "history_minimize",
                  "history_minimize_hover",
                  "history_minimize_stop_hover",
                  "history_window_left_button_down" }

_G.history_dialog = "history_dialog"
_G.history_close_button = "close_history_button"
_G.history_minimize_button = "minimize_history_button"
_G.history_normalize_button = "history_normalize_button"
_G.button_size = 34.5

function script.on_service_menu_item_loaded_ret(args)
    local menu = require("szlua.ui.menu");
    local icons = require("szlua.media.icons")
    local localstr = require("szlua.loc.str")

    local menu_item = menu.ctor_element();
    menu_item.icon = icons.from_material("History");

    local menu_item_title = localstr.ctor();
    menu_item_title:on("ru-RU", "История проводника");
    menu_item_title:default("Explorer history");
    menu_item.title = menu_item_title:build();

    menu_item.onclick_event = "history_menu_item_onclick"

    return menu_item:build():register()
end

function script.on_history_menu_item_onclick(args)
    local dialog = require("szlua.ui.dialog")
    local locstr = require("szlua.loc.str")
    local grid = require("szlua.ui.grid")
    local stack_panel = require("szlua.ui.stackpanel")
    local button = require("szlua.ui.button")
    local icons = require("szlua.media.icons")

    local title = locstr.ctor()
    title:on("ru-RU", "История проводника")
    title:default("Explorer history");

    local dialog_instance = dialog.ctor()

    dialog_instance:off_default_style()
    dialog_instance.title = title:build()

    dialog_instance.mouse_left_button_down_event = "history_window_left_button_down"

    local grid = grid.ctor()
    grid:row_def("auto", "*", "auto")

    local title_bar_grid = grid.ctor()
    title_bar_grid.column_def("auto, *")

    local dialog_name_and_icon = stack_panel.ctor()
    dialog_name_and_icon:set_orientation("horizontal")
    dialog_name_and_icon:build()
    title_bar_grid:set_column(1, dialog_name_and_icon)

    local buttons = stack_panel.ctor()
    buttons:set_orientation("horizontal")
    buttons.horizontal_alignment = "right"

    local close = button.ctor()
    close:set_icon(icons.from_material("Close", _G.button_size, _G.button_size))
    close.horizontal_alignment = "right"
    close.onclick_event = "history_close"
    close.mouse_enter_event = "history_close_hover"
    close.mouse_leave_event = "history_close_stop_hover"
    close:to_shared(script.shared, _G.history_close_button)

    local minimize = button.ctor()
    minimize:set_icon(icons.from_material("Minimize", _G.button_size, _G.button_size))
    minimize.horizontal_alignment = "right"
    minimize.onclick_event = "history_minimize"
    minimize.mouse_enter_event = "history_minimize_hover"
    minimize.mouse_leave_event = "history_minimize_stop_hover"
    minimize:to_shared(script.shared, _G.history_minimize_button)

    local normalize = button.ctor()
    normalize:set_icon(icons.from_material("Maximize", _G.button_size, _G.button_size))
    normalize.horizontal_alignment = "right"
    normalize.onclick_event = "history_normalize"
    normalize.mouse_enter_event = "history_normalize_hover"
    normalize.mouse_leave_event = "history_normalize_stop_hover"
    normalize:to_shared(script.shared, _G.history_normalize_button)

    buttons:add(minimize:build())
    buttons:add(normalize:build())
    buttons:add(close:build())

    title_bar_grid:set_column(2, buttons:build())
    title_bar_grid:build()
    grid:set_row(1, title_bar_grid)

    dialog_instance:set_content(grid)

    dialog_instance:build()
    dialog_instance:to_shared(script.shared, _G.history_dialog)
    dialog_instance:show()
end

function script.on_history_window_left_button_down(args)
    local dialog = require("szlua.ui.dialog")

    local dialog_instance = dialog.from_shared(script.shared, _G.history_dialog)
    dialog_instance:drag_move()
end

function script.on_history_close_hover(args)
    local themes = require("szlua.media.themes")
    local button = require("szlua.ui.button")
    local close = button.from_shared(script.shared, _G.history_close_button)
    close.foreground = themes.PrimaryColor
    close:build()
end

function script.on_history_close_stop_hover(args)
    local themes = require("szlua.media.themes")
    local button = require("szlua.ui.button")
    local close = button.from_shared(script.shared, _G.history_close_button)
    close.foreground = themes.ForegroundColor
    close:build()
end

function script.on_history_minimize_hover(args)
    local themes = require("szlua.media.themes")
    local button = require("szlua.ui.button")
    local minimize = button.from_shared(script.shared, _G.history_minimize_button)
    minimize.foreground = themes.ForegroundHoverColor
    minimize:build()
end

function script.on_history_minimize_stop_hover(args)
    local themes = require("szlua.media.themes")
    local button = require("szlua.ui.button")
    local minimize = button.from_shared(script.shared, _G.history_minimize_button)
    minimize.foreground = themes.ForegroundColor
    minimize:build()
end

function script.on_history_normalize_hover(args)
    local themes = require("szlua.media.themes")
    local button = require("szlua.ui.button")
    local normalize = button.from_shared(script.shared, _G.history_normalize_button)
    normalize.foreground = themes.ForegroundHoverColor
    normalize:build()
end

function script.on_history_normalize_stop_hover(args)
    local themes = require("szlua.media.themes")
    local button = require("szlua.ui.button")
    local normalize = button.from_shared(script.shared, _G.history_normalize_button)
    normalize.foreground = themes.ForegroundColor
    normalize:build()
end

function script.on_history_minimize()
    local dialog = require("szlua.ui.dialog")

    local dialog_instance = dialog.from_shared(script.shared, _G.history_dialog)
    dialog_instance:minimize()
end

function script.on_history_normalize()
    local dialog = require("szlua.ui.dialog")

    local dialog_instance = dialog.from_shared(script.shared, _G.history_dialog)
    dialog_instance:normalize()
end

function script.on_history_close()
    local dialog = require("szlua.ui.dialog")

    local dialog_instance = dialog.from_shared(script.shared, _G.history_dialog)
    dialog_instance:close()
    script.logger.info("Closing explorer history dialog")
end

function script.on_silver_pack1_description_ret(args)
    return {
        name = "ExplorerHistory",
        version = "1.0.0",
        description = "Adding a menu item for showing explorer history"
    }
end

