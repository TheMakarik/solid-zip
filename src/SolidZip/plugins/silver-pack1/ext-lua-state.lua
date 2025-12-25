script.events = {'startup', 'exit', 'silver_pack1_description_ret', 'show_startup_script'}

function script.on_startup(args)
    script.logger.info("Language version: " .. _VERSION)
    if script.shared.sp1_indev then
        script.debug.print(_VERSION .. " was started(debug test)")
        local resources = require("szlua.ui.resources")
        script.debug.print(resources.BackgroundColorBrush:ToString())
        
        local events = require("szlua.events");
        events.raise("show_startup_script")
        for k, _ in pairs(luanet) do
            script.debug.print("luanet function: " .. k);
        end
    end
end

function script.on_exit(args)
    local message = "Application is closing with code: "
    script.logger.info(message .. tostring(args))
    if script.shared.sp1_indev then
        script.debug.print(message .. tostring(args))
    end
end

function script.on_show_startup_script(args)
    local events = require("szlua.events");
    local res = events.get_scripts("startup")
    script.debug.print("Startup events: " .. getval_asstr(res) )
end


function script.on_silver_pack1_description_ret(args)
    return {
        name = "LuaState",
        version = "1.0.0",
        description = "Simple lua extension, to log version at start and log that lua is stopping on end"
    }
end

function getval_asstr(t)
    local result = ""
    for _, v in pairs(t) do
        result = result .. v or ""
    end
    return result
end