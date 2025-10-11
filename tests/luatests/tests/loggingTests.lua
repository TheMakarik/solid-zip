local luaunit = require("luaunit")
local logger = require("sz-logging")

local caller = {
    infoCalled = false,
    traceCalled = false,
    debugCalled = false,
    warnCalled = false,
    errorCalled = false,
    criticalCalled = false,
    lastMessage = nil
}

_G._info = function(message)
    caller.infoCalled = true
    caller.lastMessage = message
end

_G._debug = function(message)
    caller.debugCalled = true
    caller.lastMessage = message
end

_G._trace = function(message)
    caller.traceCalled = true
    caller.lastMessage = message
end

_G._warn = function(message)
    caller.warnCalled = true
    caller.lastMessage = message
end

_G._error = function(message)
    caller.errorCalled = true
    caller.lastMessage = message
end

_G._critical = function(message)
    caller.criticalCalled = true
    caller.lastMessage = message
end

-- Функция для сброса состояния caller перед каждым тестом
local function resetCaller()
    caller.infoCalled = false
    caller.traceCalled = false
    caller.debugCalled = false
    caller.warnCalled = false
    caller.errorCalled = false
    caller.criticalCalled = false
    caller.lastMessage = nil
end

function test_info_invokePrivateMember_info()
    --arrange
    resetCaller()

    --act
    logger.info("INFO_TEST")

    --assert
    luaunit.assertEquals(caller.infoCalled, true)
    luaunit.assertEquals(caller.lastMessage, "INFO_TEST")
end

function test_debug_invokePrivateMember_debug()
    --arrange
    resetCaller()

    --act
    logger.debug("DEBUG_TEST")

    --assert
    luaunit.assertEquals(caller.debugCalled, true)
    luaunit.assertEquals(caller.lastMessage, "DEBUG_TEST")
end

function test_trace_invokePrivateMember_trace()
    --arrange
    resetCaller()

    --act
    logger.trace("TRACE_TEST")

    --assert
    luaunit.assertEquals(caller.traceCalled, true)
    luaunit.assertEquals(caller.lastMessage, "TRACE_TEST")
end

function test_warn_invokePrivateMember_warn()
    --arrange
    resetCaller()

    --act
    logger.warn("WARN_TEST")

    --assert
    luaunit.assertEquals(caller.warnCalled, true)
    luaunit.assertEquals(caller.lastMessage, "WARN_TEST")
end

function test_error_invokePrivateMember_error()
    --arrange
    resetCaller()

    --act
    logger.error("ERROR_TEST")

    --assert
    luaunit.assertEquals(caller.errorCalled, true)
    luaunit.assertEquals(caller.lastMessage, "ERROR_TEST")
end

function test_critical_invokePrivateMember_critical()
    --arrange
    resetCaller()

    --act
    logger.critical("CRITICAL_TEST")

    --assert
    luaunit.assertEquals(caller.criticalCalled, true)
    luaunit.assertEquals(caller.lastMessage, "CRITICAL_TEST")
end

function test_multiple_calls_track_last_message()
    --arrange
    resetCaller()

    --act
    logger.info("FIRST_MESSAGE")
    logger.error("SECOND_MESSAGE")
    logger.warn("THIRD_MESSAGE")

    --assert
    luaunit.assertEquals(caller.lastMessage, "THIRD_MESSAGE")
    luaunit.assertEquals(caller.infoCalled, true)
    luaunit.assertEquals(caller.errorCalled, true)
    luaunit.assertEquals(caller.warnCalled, true)
end

os.exit(luaunit.LuaUnit.run())