script.events = {'startup', 'description_ret'}

function script.on_startup(args)
    script.logger.info("Language version: " .. _VERSION)
end

function script.on_description_ret(args)
    return {
        name = "LuaState",
        version = "1.0.0",
        description = "Simple lua extension, to log version at start and log that lua is stopping on end"
    }
end 