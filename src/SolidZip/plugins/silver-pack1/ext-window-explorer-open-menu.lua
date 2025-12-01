script.events = {'file_menu_item_loaded_ret', 'explorer_context_menu_loaded_ret', 'silver_pack1_description_ret'}

function generate_menu(path_to_open_function)
    local menu = require("szlua\\ui\\menu");
    local icons = require("szlua\\media\\icons")
    local localstr = require("szlua\\loc\\str")

    local menu_item = menu.ctor_element();
    menu_item.icon = icons.from_material("DesktopWindows");

    local menu_item_title = localstr.ctor();
    menu_item_title:on("ru-RU", "Открыть с помощью Windows Explorer");
    menu_item_title:on("", "Open in Windows Explorer");
    menu_item.title = menu_item_header:build();


    menu_item.onclick = function(args) handle_onclick(path_to_open_function(), args) end
  
    return menu_item:build();
end


function handle_onclick(path_to_open, args)
    local command = "start /min \"\" ";

    if type(path_to_open) == "table" then
        for _, path in ipairs(path_to_open) do
            local message = "Opening in windows explorer: " .. path
            
            if script.shared.sp1_indev then
                script.debug.print(message)
            end
            
            script.logger.info(message)
            os.execute(command .. script.folder .. "\\bat\\windows-explorer-open.bat" .. path)
        end

    elseif type(path_to_open) == "string" then
        local message = "Opening in windows explorer: " .. path_to_open

        if script.shared.sp1_indev then
            script.debug.print(message)
        end
     
        script.logger.info(message)
        os.execute(command .. script.folder .. "\\bat\\windows-explorer-open.bat" .. path_to_open)
    end
end

function script.on_file_menu_loaded_ret(args)
    return generate_menu(function() return script.ui.select_items_path or {} end);
end 

function script.on_explorer_context_menu_loaded_ret(args)
    return generate_menu(function() return script.ui.context_menu_opened_path or {} end)
end

function script.on_silver_pack1_description_ret(args)
    return {
        name = "OpenInWindowsExplorerMenu",
        version = "1.0.0",
        description = "Adding a menu item for opening folder or file in windows explorer"
    }
end

