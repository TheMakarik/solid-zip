script.events = {'service_menu_item_loaded_ret', 'silver_pack1_description_ret'}

function script.on_service_menu_item_loaded_ret(args)
    local menu = require("szlua.ui.menu");
    local icons = require("szlua.media.icons")
    local localstr = require("szlua.loc.str")

    local menu_item = menu.ctor_element();
    menu_item.icon = icons.from_material("History");

    local menu_item_title = localstr.ctor();
    menu_item_title:on("ru-RU", "История проводника");
    menu_item_title:on("", "Explorer History");
    menu_item.title = menu_item_title:build();


    menu_item.onclick = function(args) end

    return menu_item:build();
end


function script.on_silver_pack1_description_ret(args)
    return {
        name = "ExplorerHistory",
        version = "1.0.0",
        description = "Adding a menu item for showing explorer history"
    }
end

