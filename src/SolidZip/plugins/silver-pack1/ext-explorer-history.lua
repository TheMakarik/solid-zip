script.events = {'service_menu_item_loaded_ret', 
                 'silver_pack1_description_ret', 
                 "history_menu_item_onclick", 
                 "history_window_left_button_down"}

_G.history_dialog = "history_dialog"

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

    return menu_item:build();
end

function script.on_history_menu_item_onclick(args)
    local dialog = require("szlua.ui.dialog")
    local locstr = require("szlua.loc.str")
    local grid = require("szlua.ui.grid")
    local stack_panel = require("szlua.ui.stackpanel")
    
    local title = locstr.ctor()
    title:on("ru-RU", "История проводника")
    title:default("Explorer history");
    
    local dialog_instance = dialog.ctor()
    
    dialog_instance:off_default_style()
    dialog_instance.title = title:build()
    
    dialog_instance.mouse_left_button_down_event = "history_window_left_button_down"
    
    local grid = grid.ctor()
    grid:row_def("auto", "*", "auto")
    --[[
    local title_bar_grid = grid.ctor()
    title_bar_grid.column_def("auto, *")
    local dialog_name_and_icon = stack_panel.ctor()
    dialog_name_and_icon:set_orientation("horizontal")
    
    title_bar_grid.set_row(1, dialog_name_and_icon:build())
    local buttons = stack_panel.ctor()
    grid.set_row(1, title_bar_grid:build())
    ]]
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
end

function script.on_silver_pack1_description_ret(args)
    return {
        name = "ExplorerHistory",
        version = "1.0.0",
        description = "Adding a menu item for showing explorer history"
    }
end

