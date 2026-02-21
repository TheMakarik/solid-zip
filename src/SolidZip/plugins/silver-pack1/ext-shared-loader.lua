script.events = { 'init', 'silver_pack1_description_ret' }

function script.on_init(args)
    _G.sp1_indev = true
    script.logger.debug("Shared loaded: " .. "sp1_indev=" .. tostring(script.shared.sp1_indev))
end
function script.on_silver_pack1_description_ret(args)
    return {
        name = "LuaSharedLoader",
        version = "1.0.0",
        description = "Loading lua shared data for silver-pack1 plugin"
    }
end 