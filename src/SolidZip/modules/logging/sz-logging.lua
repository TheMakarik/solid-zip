local logger = {};

---@param text string text to write
---Write log message with 'Information' level
function logger.info(text)
    _G._info(text);
end

---@param text string text to write
---Write log message with 'Debug' level
function logger.debug(text)
    _G._debug(text);
end

---@param text string text to write
---Write log message with 'Error' level
function logger.error(text)
    _G._error(text);
end

---@param text string text to write
---Write log message with 'Warning' level
function logger.warn(text)
    _G._warn(text);
end

---@param text string text to write
---Write log message with 'Critical' level
function logger.critical(text)
    _G._critical(text);
end

---@param text string text to write
---Write log message with 'Trace' level
function logger.trace(text)
    _G._trace(text);
end

return logger;