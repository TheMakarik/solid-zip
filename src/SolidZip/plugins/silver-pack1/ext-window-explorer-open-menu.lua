script.events = {'file_menu_opened_ret', 'explorer_context_menu_opened_ret', 'silver_pack1_description_ret'}

function generate_menu(path_to_open)
    local menu = require("szlua\\ui\\menu");
    local icons = require("szlua\\media\\icons")
    local localstr = require("szlua\\loc\\str")

    local menu_item = menu.ctor_element();
    menu_item.icon = icons.from_material("DesktopWindows");
    menu_item.onclick = function(args)
        local script_folder = get_script_path();
        local command = "start /min \"\" ";
        
        if type(path_to_open) == "table" then
            for _, path in ipairs(path_to_open) do
                local message = "Opening in windows explorer: " .. path
                script.debug.print(message)
                script.logger.info(message)
                os.execute(command .. script_folder .. "\\bat\\windows-explorer-open.bat" .. path)
            end
         
        elseif type(path_to_open) == "string" then
            local message = "Opening in windows explorer: " .. path_to_open
            script.debug.print(message)
            script.logger.info(message)
            os.execute(command .. script_folder .. "\\bat\\windows-explorer-open.bat" .. path_to_open)
        end
    end
    local menu_item_header = localstr.ctor();
    menu_item_header:on("ru-RU", "Открыть с помощью Windows Explorer");
    menu_item_header:on("", "Open in Windows Explorer");
    menu_item.header = menu_item_header:build();
    
    return menu_item:build();
end

function get_script_folder()
    local info = debug.getinfo(1, "S")
    local path = info.source:match("@?(.*)")
    
    if path:sub(1, 1) == "@" then
        path = path:sub(2)
    end
    
    local directory = path:match("(.*)[/\\]") or "."
    directory = directory:gsub("[/\\]+$", "")

    return directory
end

function script.file_menu_opened_ret(args)
    return generate_menu(script.shared.select_items);
end 

function script.explorer_context_menu_opened_ret(args)
    if script.shared.explorer_context_menu_entity_path ~= nil then
        return generate_menu(script.shared.explorer_context_menu_entity_path);
    end
end

function script.on_silver_pack1_description_ret(args)
    return {
        name = "OpenInWindowsExplorerMenu",
        version = "1.0.0",
        description = "Adding a menu item for opening folder or file in windows explorer"
    }
end

