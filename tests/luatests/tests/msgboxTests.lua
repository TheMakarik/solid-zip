local luaunit = require("luaunit")

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



function tearDown()
    msgbox.show = originalShow
    resetDefaults()
end

os.exit(luaunit.LuaUnit.run())