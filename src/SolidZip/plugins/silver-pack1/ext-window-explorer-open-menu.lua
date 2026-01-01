script.events = {'file_menu_item_loaded_ret',
                 'explorer_context_menu_loaded_ret', 
                 'windows_explorer_item_onclick',
                 'silver_pack1_description_ret'}

function script.on_file_menu_item_loaded_ret(args)
    local menu = require("szlua.ui.menu")
    local locstr = require("szlua.loc.str")
    local icons = require("szlua.media.icons")
    
    local item_title = locstr.ctor()
    item_title:on("ru-RU", "Показать в Windows Explorer")
    item_title:default("Show via Windows Explorer")
    
    
    local item = menu.ctor_element()
    item.title = item_title:build()
    item.icon = icons.from_material('MicrosoftWindowsClassic')
    item.onclick_event = "windows_explorer_item_onclick"
    
    local tooltip = locstr.ctor()
    tooltip:on("ru-RU", "Показывает выделенные файлы в Windows Explorer")
    tooltip:default("Show selected files in Windows Explorer")
    item.tooltip = tooltip:build()

    if script.shared.sp1_indev then
        script.debug.print("Creating Windows Explorer menu item")
    end
    script.logger.debug("Creating Windows Explorer menu item")
    
    return item:build():register()
end 

function script.on_windows_explorer_item_onclick(args)
    local command = "explorer ";
    
    for _, v in ipairs(script.ui.selected_entities_path or {}) do
        if script.shared.sp1_indev then
            script.debug.print("Opening in windows explorer: " .. v)
        end
        os.execute(command  .. v)
    end
   
end

function script.on_explorer_context_menu_loaded_ret(args)
    return generate_menu("context_menu")
end

function script.on_silver_pack1_description_ret(args)
    return {
        name = "OpenInWindowsExplorerMenu",
        version = "1.0.0",
        description = "Adding a menu item for opening folder or file in windows explorer"
    }
end

