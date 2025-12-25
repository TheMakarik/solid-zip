script.events = {"file_menu_item_loaded_ret", "history_onclick"}

function script.on_file_menu_item_loaded_ret(args)
    local icons = require("szlua.media.icons")
    local menu = require("szlua.ui.menu")

    local menu_item = menu.ctor_element()
    menu_item.icon = icons.from_material("History")
    menu_item.title = "Dialog tests"
    menu_item.onclick_event = "history_onclick"

    return menu_item:build()
end

function script.on_history_onclick(args)
    local dialog = require("szlua.ui.dialog")
    local dialog_history = dialog.ctor()
    dialog_history:build() --Создание контрола
    dialog_history:show() --Вывод контрола на экран
end 