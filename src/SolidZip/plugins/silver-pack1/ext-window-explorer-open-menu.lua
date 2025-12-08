script.events = {'file_menu_item_loaded_ret',
                 'explorer_context_menu_loaded_ret', 
                 'windows_explorer_opener_onclick',
                 'silver_pack1_description_ret'}

function generate_menu(loaded_from)
    local menu = require("szlua\\ui\\menu");
    local icons = require("szlua\\media\\icons")
    local localstr = require("szlua\\loc\\str")

    script.logger.debug("Loading OpenInWindowsExplorer menu item from ".. loaded_from);
    
    local menu_item = menu.ctor_element();
    menu_item.icon = icons.from_material("MicrosoftWindowsClassic");

    local menu_item_title = localstr.ctor();
    menu_item_title:on("ru-RU", "Открыть с помощью Windows Explorer");
    menu_item_title:on("", "Open in Windows Explorer");
    menu_item.title = menu_item_title:build();


    menu_item.onclick_event = "windows_explorer_opener_onclick";
    menu_item.onclick_args = {loaded_from = loaded_from}
  
    return menu_item:build();
end


function script.on_windows_explorer_opener_onclick( args)
    local command = "start /min \"\" ";

    if script.shared.sp1_indev then
        script.debug.print("'Show in Windows explorer button was click'");
    end

    if args.loaded_from == "menu" then
        for _, path in ipairs(script.ui.selected_items_path) do
            local message = "Opening in windows explorer: " .. path
            
            if script.shared.sp1_indev then
                script.debug.print(message)
            end
            
            script.logger.info(message)
            os.execute(command .. script.folder .. "\\bat\\windows-explorer-open.bat" .. path)
        end

    elseif args.loaded_from == "context_menu" then
        local message = "Opening in windows explorer: " .. path_to_open

        if script.shared.sp1_indev then
            script.debug.print(message)
        end
     
        script.logger.info(message)
        os.execute(command .. script.folder .. "\\bat\\windows-explorer-open.bat" .. script.ui.context_menu_opened_path)
    end
end

function script.on_file_menu_item_loaded_ret(args)
    return generate_menu("menu");
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

