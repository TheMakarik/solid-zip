local grid = {}

local ui_element = require("szlua.ui.sz_ui_element")

if _G.import ~= nil then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

function grid.ctor()
    local grid_instance = {}
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        grid_instance._wpf_grid = Grid()
    end)
    return setmetatable(grid_instance, {__index = grid})
end

function grid.from_shared(shared, name)
    local converter = require("szlua.private.converter")
    local result = converter.dotnet_dict_to_table(shared[name])
    result._wpf_grid = shared[name .. "_control"]
    return setmetatable(result, {__index = grid})
end

function grid:to_shared(shared, name)
    local converter = require("szlua.private.converter")
    shared[name .. "_control"] = self._wpf_grid
    shared[name] = converter.table_to_dotnet_dict(self)
end

function grid:row_def(...)
    local args = {...}
    local dispatcher = require("szlua.ui.dispatcher")

    for _, def in ipairs(args) do
        assert(type(def) == "number" or type(def) == "string", "row_def arguments must be string or number, got " .. type(def))

        dispatcher.exec(function()
            local row = RowDefinition()

            if type(def) == "string" then
                local clean_def = string.gsub(def, "%s+", "")

                if clean_def:lower() == "auto" then
                    row.Height = GridLength.Auto
                elseif string.sub(clean_def, -1) == "*" then
                    if #clean_def == 1 then
                        row.Height = GridLength(1, GridUnitType.Star)
                    else
                        local num = tonumber(string.sub(clean_def, 1, -2))
                        if num then
                            row.Height = GridLength(num, GridUnitType.Star)
                        else
                            error("Invalid star value: " .. clean_def)
                        end
                    end
                else
                    local num = tonumber(clean_def)
                    if num then
                        row.Height = GridLength(num)
                    else
                        error("unexpected grid row height parameter: " .. def)
                    end
                end
            else
                row.Height = GridLength(def)
            end

            self._wpf_grid.RowDefinitions:Add(row)
        end)
    end
end

function grid:column_def(...)
    local args = {...}
    local dispatcher = require("szlua.ui.dispatcher")

    for i, def in ipairs(args) do
        assert(type(def) == "number" or type(def) == "string",
                "column_def arguments must be string or number, got " .. type(def))

        dispatcher.exec(function()
            local column = ColumnDefinition()

            if type(def) == "string" then
                local clean_def = string.gsub(def, "%s+", "")

                if clean_def:lower() == "auto" then
                    column.Width = GridLength.Auto
                elseif string.sub(clean_def, -1) == "*" then
                    if #clean_def == 1 then
                        column.Width = GridLength(1, GridUnitType.Star)
                    else
                        local num = tonumber(string.sub(clean_def, 1, -2))
                        if num then
                            column.Width = GridLength(num, GridUnitType.Star)
                        else
                            error("Invalid star value: " .. clean_def)
                        end
                    end
                else
                    local num = tonumber(clean_def)
                    if num then
                        column.Width = GridLength(num)
                    else
                        error("unexpected grid column width parameter: " .. def)
                    end
                end
            else
                column.Width = GridLength(def)
            end

            self._wpf_grid.ColumnDefinitions:Add(column)
        end)
    end
end

function grid:set_row(row, content)
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_grid.Children:Add(content:register())
        Grid.SetRow(content:register(), row - 1)
    end)
end

function grid:set_column(column, content)
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_grid.Children:Add(content:register())
        Grid.SetColumn(content:register(), column - 1)
    end)
end

function grid:set_row_span(row, content)
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_grid:SetRowSpan(content:register(), row)
    end)
end

function grid:set_column_span(column, content)
    local dispatcher = require("szlua.ui.dispatcher")
    dispatcher.exec(function()
        self._wpf_grid:SetColumnSpan(content:register(), column)
    end)
end

function grid:build()
    ui_element.register_base(self._wpf_grid, self)
    return self
end

function grid:register()
    return self._wpf_grid
end

return grid