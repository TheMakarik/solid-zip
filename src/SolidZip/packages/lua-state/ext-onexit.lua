script = {};

script.events = {"EXIT"}

function script.execute()
    local logger = require("sz-logging");
    logger.info("Close application log from lua code");
end

