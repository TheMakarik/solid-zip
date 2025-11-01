local json = {}

if _G.import ~= nil then
    import('System.Text.Json')
    import('System.Text.Json.Nodes')
    import('System.IO')
end

---@class JsonData
---Represents a JSON object with dynamic property access

---Parses a JSON string and returns a manipulatable JSON object
---@param jsonString string The JSON string to parse
---@return JsonData|nil # JSON object or nil if parsing fails
---@return string|nil # Error message if parsing fails
function json.parseString(jsonString)
    if jsonString == nil or type(jsonString) ~= 'string' then
        return nil, "Invalid JSON string"
    end

    local success, result = pcall(function()
        local jsonNode = System.Text.Json.Nodes.JsonNode.Parse(jsonString)
        return jsonNode
    end)

    if not success then
        return nil, "Failed to parse JSON: " .. tostring(result)
    end

    return json.createProxy(result)
end

---Parses a JSON file and returns a manipulatable JSON object
---@param filePath string Path to the JSON file
---@return JsonData|nil # JSON object or nil if parsing fails
---@return string|nil # Error message if parsing fails
function json.parseFile(filePath)
    if filePath == nil or type(filePath) ~= 'string' then
        return nil, "Invalid file path"
    end

    local success, fileContent = pcall(function()
        return System.IO.File.ReadAllText(filePath)
    end)

    if not success then
        return nil, "Failed to read file: " .. tostring(fileContent)
    end

    return json.parseString(fileContent)
end

---Serializes a JSON object back to string
---@param jsonObj JsonData The JSON object to serialize
---@param indent? boolean Whether to format with indentation (default: false)
---@return string|nil # JSON string or nil if serialization fails
---@return string|nil # Error message if serialization fails
function json.serialize(jsonObj, indent)
    if jsonObj == nil or jsonObj.__node == nil then
        return nil, "Invalid JSON object"
    end

    indent = indent or false

    local success, result = pcall(function()
        if indent then
            local options = System.Text.Json.JsonSerializerOptions({
                WriteIndented = true
            })
            return jsonObj.__node:ToJsonString(options)
        else
            return jsonObj.__node:ToJsonString()
        end
    end)

    if not success then
        return nil, "Failed to serialize JSON: " .. tostring(result)
    end

    return result
end

---Writes JSON object to file
---@param jsonObj JsonData The JSON object to write
---@param filePath string Path to the output file
---@param indent? boolean Whether to format with indentation (default: false)
---@return boolean # Success status
---@return string|nil # Error message if failed
function json.writeToFile(jsonObj, filePath, indent)
    local jsonString, err = json.serialize(jsonObj, indent)
    if not jsonString then
        return false, err
    end

    local success, result = pcall(function()
        System.IO.File.WriteAllText(filePath, jsonString)
        return true
    end)

    if not success then
        return false, "Failed to write file: " .. tostring(result)
    end

    return true
end

---Creates a new empty JSON object
---@return JsonData # New empty JSON object
function json.newObject()
    local success, jsonNode = pcall(function()
        return System.Text.Json.Nodes.JsonObject()
    end)

    if not success then
        error("Failed to create JSON object: " .. tostring(jsonNode))
    end

    return json.createProxy(jsonNode)
end

---Creates a new JSON array
---@return JsonData # New JSON array
function json.newArray()
    local success, jsonNode = pcall(function()
        return System.Text.Json.Nodes.JsonArray()
    end)

    if not success then
        error("Failed to create JSON array: " .. tostring(jsonNode))
    end

    return json.createProxy(jsonNode)
end

---Internal function to create proxy object for JSON node with metamethods
---@param jsonNode userdata The System.Text.Json.Nodes.JsonNode object
---@return JsonData # Proxy object with metamethods
function json.createProxy(jsonNode)
    local proxy = {
        __node = jsonNode,
        __type = jsonNode:GetType():ToString()
    }
    
    local isArray = proxy.__type:find("JsonArray") ~= nil

    setmetatable(proxy, {
        __index = function(self, key)
            -- Handle numeric indexing for arrays
            if isArray and type(key) == "number" then
                local success, value = pcall(function()
                    local element = self.__node[key - 1] -- C# arrays are 0-based
                    if element ~= nil then
                        return json.createProxy(element)
                    end
                    return nil
                end)
                return success and value or nil
            end
            
            local success, value = pcall(function()
                local prop = self.__node[key]
                if prop ~= nil then
                    return json.createProxy(prop)
                end
                return nil
            end)

            if success then
                return value
            end
            
            local success2, rawValue = pcall(function()
                if self.__node.GetValue then
                    local valueType = self.__node:GetValue():GetType():ToString()
                    if valueType:find("String") then
                        return self.__node:GetValue(System.String)
                    elseif valueType:find("Int") or valueType:find("Double") or valueType:find("Decimal") then
                        return self.__node:GetValue(System.Double)
                    elseif valueType:find("Boolean") then
                        return self.__node:GetValue(System.Boolean)
                    end
                end
                return nil
            end)

            return success2 and rawValue or nil
        end,

        __newindex = function(self, key, value)
            if isArray and type(key) == "number" then
                local success = pcall(function()
                    local jsonValue = json.convertToJsonNode(value)
                    self.__node[key - 1] = jsonValue -- C# arrays are 0-based
                end)
                if not success then
                    error("Failed to set array element at index " .. key)
                end
            else
                local success = pcall(function()
                    local jsonValue = json.convertToJsonNode(value)
                    self.__node[key] = jsonValue
                end)
                if not success then
                    error("Failed to set property: " .. tostring(key))
                end
            end
        end,

        __len = function(self)
            if isArray then
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
            if isArray then
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

---Internal function to convert Lua values to JsonNode
---@param value any Lua value to convert
---@return userdata # JsonNode representation
function json.convertToJsonNode(value)
    if type(value) == "table" and value.__node ~= nil then
        return value.__node
    elseif type(value) == "string" then
        return System.Text.Json.Nodes.JsonValue.Create(value)
    elseif type(value) == "number" then
        return System.Text.Json.Nodes.JsonValue.Create(value)
    elseif type(value) == "boolean" then
        return System.Text.Json.Nodes.JsonValue.Create(value)
    elseif value == nil then
        local jsonNode = System.Text.Json.Nodes.JsonNode.Parse("null")
        return jsonNode
    else
        error("Unsupported value type: " .. type(value))
    end
end

return json