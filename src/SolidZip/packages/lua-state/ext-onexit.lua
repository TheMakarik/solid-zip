local script = {};

function script.subscribe()
    return {"EXIT"}
end

function script.execute()
    local logger = require("sz-logging");
    logger.info("Close application log from lua code");
end

return script;

