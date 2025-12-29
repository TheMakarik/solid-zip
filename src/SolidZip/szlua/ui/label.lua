local label = {}

local ui_element = require("szlua.ui.sz_ui_element")

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

function label.ctor()
    local label_instance = {}
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        label_instance._wpf_label = Label()
    end)
    return setmetatable(label_instance, {__index = label})
end

function label:build()
    local dispatcher = require("szlua.ui.dispatcher")
    
    if type(self.content) == "string" then
        dispatcher.exec(function()
            self._wpf_label.Content = self.content
        end)
    end
    self.background = "#00FFFFFF"
    ui_element.register_base(self._wpf_label, self)
    return self
end

function label:register()
    return self._wpf_label
end

function label.from_shared(shared, name)
    local converter = require("szlua.private.converter")
    local result =  converter.dotnet_dict_to_table(shared[name])
    result._wpf_label = shared[name .. "_control"]
    return setmetatable(result, {__index = label});
end

function label:to_shared(shared, name)
    local converter = require("szlua.private.converter")
    shared[name .. "_control"] = self:register()
    shared[name] = converter.table_to_dotnet_dict(self)
end

return label