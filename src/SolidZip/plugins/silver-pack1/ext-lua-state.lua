script.events = {'startup', 'exit', 'description_ret'}

function script.on_startup(args)
    script.logger.info("Language version: " .. _VERSION)
    if script.shared.sp1_indev then
        script.debug.print(_VERSION .. " was started(debug test)")
    end
end

function script.on_exit(args)
    local message = "Application is closing with code: "
    script.logger.info(message .. tostring(args))
    if script.shared.sp1_indev then
        script.debug.print(message .. tostring(args))
    end
end


function script.on_description_ret(args)
    return {
        name = "LuaState",
        version = "1.0.0",
        description = "Simple lua extension, to log version at start and log that lua is stopping on end"
    }
end 