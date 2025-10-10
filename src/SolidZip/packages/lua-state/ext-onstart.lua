script = {};

script.events = {"STARTUP"}

function script.execute()
    local logger = require("sz-logging");
    logger.info("Start Lua" .. tostring(_VERSION));
end 
