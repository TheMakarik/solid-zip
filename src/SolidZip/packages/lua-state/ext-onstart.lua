local script = {};

script.events = {"STARTUP"}

function script.execute()
    local logger = require("sz-logging");
    logger.info("Start application log from lua code");
end 

return script;