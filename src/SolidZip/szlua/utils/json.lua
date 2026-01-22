local json = {}

if _G.import ~= nil then
    import('System.Text.Json')
    import('System.Text.Json.Nodes')
    import('System.IO')
end

function json.parse_string(json_string)
    if json_string == nil or type(json_string) ~= 'string' then
        return nil, "Invalid JSON string"
    end

    local success, result = pcall(function()
        local json_node = System.Text.Json.Nodes.JsonNode.Parse(json_string)
        return json_node
    end)

    if not success then
        return nil, "Failed to parse JSON: " .. tostring(result)
    end

    return json.create_proxy(result)
end

function json.parse_file(file_path)
    if file_path == nil or type(file_path) ~= 'string' then
        return nil, "Invalid file path"
    end

    local success, file_content = pcall(function()
        return System.IO.File.ReadAllText(file_path)
    end)

    if not success then
        return nil, "Failed to read file: " .. tostring(file_content)
    end

    return json.parse_string(file_content)
end

function json.serialize(json_obj, indent)
    if json_obj == nil or json_obj.__node == nil then
        return nil, "Invalid JSON object"
    end

    indent = indent or false

    local success, result = pcall(function()
        if indent then
            local options = System.Text.Json.JsonSerializerOptions({
                WriteIndented = true
            })
            return json_obj.__node:ToJsonString(options)
        else
            return json_obj.__node:ToJsonString()
        end
    end)

    if not success then
        return nil, "Failed to serialize JSON: " .. tostring(result)
    end

    return result
end

function json.write_to_file(json_obj, file_path, indent)
    local json_string, err = json.serialize(json_obj, indent)
    if not json_string then
        return false, err
    end

    local success, result = pcall(function()
        System.IO.File.WriteAllText(file_path, json_string)
        return true
    end)

    if not success then
        return false, "Failed to write file: " .. tostring(result)
    end

    return true
end

function json.new_object()
    local success, json_node = pcall(function()
        return System.Text.Json.Nodes.JsonObject()
    end)

    if not success then
        error("Failed to create JSON object: " .. tostring(json_node))
    end

    return json.create_proxy(json_node)
end

function json.new_array()
    local success, json_node = pcall(function()
        return System.Text.Json.Nodes.JsonArray()
    end)

    if not success then
        error("Failed to create JSON array: " .. tostring(json_node))
    end

    return json.create_proxy(json_node)
end

d
function json.get_raw_node(json_obj)
    if json_obj and json_obj.__node then
        return json_obj.__node
    end
    return nil
end

function json.is_array(json_obj)
    if json_obj == nil or json_obj.__node == nil then
        return false
    end

    local success, result = pcall(function()
        local node_type = json_obj.__node:GetType():ToString()
        return node_type:find("JsonArray") ~= nil
    end)

    return success and result or false
end

function json.is_object(json_obj)
    if json_obj == nil or json_obj.__node == nil then
        return false
    end

    local success, result = pcall(function()
        local node_type = json_obj.__node:GetType():ToString()
        return node_type:find("JsonObject") ~= nil
    end)

    return success and result or false
end

function json.get_keys(json_obj)
    if not json.is_object(json_obj) then
        return nil
    end

    local success, keys = pcall(function()
        local key_list = {}
        for key, _ in pairs(json_obj) do
            table.insert(key_list, key)
        end
        return key_list
    end)

    return success and keys or nil
end

function json.create_proxy(json_node)
    local proxy = {
        __node = json_node,
        __type = json_node:GetType():ToString()
    }

    local is_array = proxy.__type:find("JsonArray") ~= nil

    setmetatable(proxy, {
        __index = function(self, key)
            -- Handle numeric indexing for arrays
            if is_array and type(key) == "number" then
                local success, value = pcall(function()
                    local element = self.__node[key - 1] -- C# arrays are 0-based
                    if element ~= nil then
                        return json.create_proxy(element)
                    end
                    return nil
                end)
                return success and value or nil
            end

            local success, value = pcall(function()
                local prop = self.__node[key]
                if prop ~= nil then
                    return json.create_proxy(prop)
                end
                return nil
            end)

            if success then
                return value
            end

            local success2, raw_value = pcall(function()
                if self.__node.GetValue then
                    local value_type = self.__node:GetValue():GetType():ToString()
                    if value_type:find("String") then
                        return self.__node:GetValue(System.String)
                    elseif value_type:find("Int") or value_type:find("Double") or value_type:find("Decimal") then
                        return self.__node:GetValue(System.Double)
                    elseif value_type:find("Boolean") then
                        return self.__node:GetValue(System.Boolean)
                    end
                end
                return nil
            end)

            return success2 and raw_value or nil
        end,

        __newindex = function(self, key, value)
            if is_array and type(key) == "number" then
                local success = pcall(function()
                    local json_value = json.convert_to_json_node(value)
                    self.__node[key - 1] = json_value -- C# arrays are 0-based
                end)
                if not success then
                    error("Failed to set array element at index " .. key)
                end
            else
                local success = pcall(function()
                    local json_value = json.convert_to_json_node(value)
                    self.__node[key] = json_value
                end)
                if not success then
                    error("Failed to set property: " .. tostring(key))
                end
            end
        end,

        __len = function(self)
            if is_array then
                local success, count = pcall(function()
                    return self.__node.Count
                end)
                return success and count or 0
            end
            return 0
        end,

        __tostring = function(self)
            local str, err = json.serialize(self)
            return str or "Invalid JSON object"
        end,

        __pairs = function(self)
            if is_array then
                local i = 0
                return function()
                    i = i + 1
                    if i <= #self then
                        return i, self[i]
                    end
                end
            else
                local enumerator = self.__node:GetEnumerator()
                local first = true
                return function()
                    if first then
                        first = false
                    else
                        enumerator:MoveNext()
                    end
                    if enumerator.Current.Key ~= nil then
                        return enumerator.Current.Key, self[enumerator.Current.Key]
                    end
                end
            end
        end
    })

    return proxy
end

function json.convert_to_json_node(value)
    if type(value) == "table" and value.__node ~= nil then
        return value.__node
    elseif type(value) == "string" then
        return System.Text.Json.Nodes.JsonValue.Create(value)
    elseif type(value) == "number" then
        return System.Text.Json.Nodes.JsonValue.Create(value)
    elseif type(value) == "boolean" then
        return System.Text.Json.Nodes.JsonValue.Create(value)
    elseif value == nil then
        local json_node = System.Text.Json.Nodes.JsonNode.Parse("null")
        return json_node
    else
        error("Unsupported value type: " .. type(value))
    end
end

return json