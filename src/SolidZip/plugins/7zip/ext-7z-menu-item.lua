script.events = {"create_new_archive_menu_item_loaded_ret", "7zip_description_ret"}

--[[
    Creates the menu item for creating new zip archive in application menu
    Icon: NumericSevenCircleOutline
]]
function script.on_create_new_archive_menu_item_loaded_ret(args)
    script.debug.print("Loading 7zip menu item")
    local menu = require("szlua.ui.menu")
    local locstr = require("szlua.loc.str")
    local icons = require("szlua.media.icons")
    
    local seven_zip_item = menu.ctor_element()
    
    local title = locstr.ctor()
    title:on("ru-RU", "7-zip Архив")
    title:default("7-zip archive")
    seven_zip_item.title = title:build()

    seven_zip_item.icon = icons.from_material("NumericSevenCircleOutline")
    
    return seven_zip_item:build():register()
end 