local luaunit = require("luaunit")

-- Сначала создаем глобальные переменные которые ожидает sz-utils-msgbox
MessageBoxImage = {
    None = 0,
    Error = 1,
    Warning = 2,
    Information = 3,
    Question = 4,
    Stop = 5,
    Exclamation = 6
}

MessageBoxButton = {
    OK = 0,
    OKCancel = 1,
    YesNo = 2,
    YesNoCancel = 3
}

MessageBoxResult = {
    None = 0,
    OK = 1,
    Cancel = 2,
    Yes = 3,
    No = 4
}

MessageBox = {
    Show = function(message, title, buttons, icon)
        return MessageBoxResult.OK
    end
}

-- Теперь подключаем модуль
local msgbox = require("sz-utils-msgbox")

local caller = {
    showCalled = false,
    lastMessage = nil,
    lastTitle = nil,
    lastButtons = nil,
    lastIcon = nil,
    returnValue = nil
}

local originalShow = msgbox.show
local originalDefaults = {
    message = msgbox.defaults.message,
    title = msgbox.defaults.title,
    buttons = msgbox.defaults.buttons,
    icon = msgbox.defaults.icon
}

msgbox.show = function(message, title, buttons, icon)
    caller.showCalled = true
    caller.lastMessage = message or msgbox.defaults.message
    caller.lastTitle = title or msgbox.defaults.title
    caller.lastButtons = buttons or msgbox.defaults.buttons
    caller.lastIcon = icon or msgbox.defaults.icon

    if caller.lastButtons == msgbox.buttons.yesNo then
        return msgbox.results.yes
    elseif caller.lastButtons == msgbox.buttons.okCancel then
        return msgbox.results.ok
    else
        return msgbox.results.ok
    end
end

local function resetCaller()
    caller.showCalled = false
    caller.lastMessage = nil
    caller.lastTitle = nil
    caller.lastButtons = nil
    caller.lastIcon = nil
    caller.returnValue = nil
end

local function resetDefaults()
    msgbox.defaults.message = originalDefaults.message
    msgbox.defaults.title = originalDefaults.title
    msgbox.defaults.buttons = originalDefaults.buttons
    msgbox.defaults.icon = originalDefaults.icon
end

function test_show_basic_message_box()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.show("TEST_MESSAGE", "TEST_TITLE", msgbox.buttons.ok, msgbox.icons.information)

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "TEST_MESSAGE")
    luaunit.assertEquals(caller.lastTitle, "TEST_TITLE")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.ok)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.information)
    luaunit.assertEquals(result, msgbox.results.ok)
end

function test_show_with_default_values()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.show("TEST_MESSAGE")

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "TEST_MESSAGE")
    luaunit.assertEquals(caller.lastTitle, "Message")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.ok)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.none)
end

function test_show_question()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.showQuestion("QUESTION_TEXT", "CUSTOM_TITLE")

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "QUESTION_TEXT")
    luaunit.assertEquals(caller.lastTitle, "CUSTOM_TITLE")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.yesNo)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.question)
    luaunit.assertEquals(result, msgbox.results.yes)
end

function test_show_warn()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.showWarn("WARNING_TEXT")

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "WARNING_TEXT")
    luaunit.assertEquals(caller.lastTitle, "Warning")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.ok)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.warning)
end

function test_show_info()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.showInfo("INFO_TEXT", "INFO_TITLE")

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "INFO_TEXT")
    luaunit.assertEquals(caller.lastTitle, "INFO_TITLE")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.ok)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.information)
end

function test_show_error()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.showError("ERROR_TEXT")

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "ERROR_TEXT")
    luaunit.assertEquals(caller.lastTitle, "Error")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.ok)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.error)
end

function test_show_confirm()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.showConfirm("CONFIRM_TEXT", "CONFIRM_TITLE")

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "CONFIRM_TEXT")
    luaunit.assertEquals(caller.lastTitle, "CONFIRM_TITLE")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.yesNo)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.question)
    luaunit.assertEquals(result, msgbox.results.yes)
end

function test_show_critical()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.showCritical("CRITICAL_TEXT")

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "CRITICAL_TEXT")
    luaunit.assertEquals(caller.lastTitle, "Critical Error")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.ok)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.stop)
end

function test_show_exclamation()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    local result = msgbox.showExclamation("EXCLAMATION_TEXT", "EXCLAMATION_TITLE")

    -- assert
    luaunit.assertEquals(caller.showCalled, true)
    luaunit.assertEquals(caller.lastMessage, "EXCLAMATION_TEXT")
    luaunit.assertEquals(caller.lastTitle, "EXCLAMATION_TITLE")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.ok)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.exclamation)
end

function test_set_defaults()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    msgbox.setDefaults("DEFAULT_MESSAGE", "DEFAULT_TITLE", msgbox.buttons.yesNoCancel, msgbox.icons.question)

    -- assert
    luaunit.assertEquals(msgbox.defaults.message, "DEFAULT_MESSAGE")
    luaunit.assertEquals(msgbox.defaults.title, "DEFAULT_TITLE")
    luaunit.assertEquals(msgbox.defaults.buttons, msgbox.buttons.yesNoCancel)
    luaunit.assertEquals(msgbox.defaults.icon, msgbox.icons.question)

    -- act
    local result = msgbox.show()

    -- assert
    luaunit.assertEquals(caller.lastMessage, "DEFAULT_MESSAGE")
    luaunit.assertEquals(caller.lastTitle, "DEFAULT_TITLE")
    luaunit.assertEquals(caller.lastButtons, msgbox.buttons.yesNoCancel)
    luaunit.assertEquals(caller.lastIcon, msgbox.icons.question)
end

function test_partial_defaults()
    -- arrange
    resetCaller()
    resetDefaults()

    -- act
    msgbox.setDefaults("INITIAL_MESSAGE", "INITIAL_TITLE", msgbox.buttons.ok, msgbox.icons.none)
    msgbox.setDefaults(nil, "ONLY_TITLE", nil, msgbox.icons.information)

    -- assert
    luaunit.assertEquals(msgbox.defaults.title, "ONLY_TITLE")
    luaunit.assertEquals(msgbox.defaults.icon, msgbox.icons.information)
    luaunit.assertEquals(msgbox.defaults.message, "INITIAL_MESSAGE")
    luaunit.assertEquals(msgbox.defaults.buttons, msgbox.buttons.ok)
end

function test_message_box_constants()
    -- arrange & act & assert
    luaunit.assertNotNil(msgbox.icons.none)
    luaunit.assertNotNil(msgbox.icons.error)
    luaunit.assertNotNil(msgbox.icons.warning)
    luaunit.assertNotNil(msgbox.icons.information)
    luaunit.assertNotNil(msgbox.icons.question)

    luaunit.assertNotNil(msgbox.buttons.ok)
    luaunit.assertNotNil(msgbox.buttons.okCancel)
    luaunit.assertNotNil(msgbox.buttons.yesNo)
    luaunit.assertNotNil(msgbox.buttons.yesNoCancel)

    luaunit.assertNotNil(msgbox.results.none)
    luaunit.assertNotNil(msgbox.results.ok)
    luaunit.assertNotNil(msgbox.results.cancel)
    luaunit.assertNotNil(msgbox.results.yes)
    luaunit.assertNotNil(msgbox.results.no)
end

function tearDown()
    msgbox.show = originalShow
    resetDefaults()
end

os.exit(luaunit.LuaUnit.run())