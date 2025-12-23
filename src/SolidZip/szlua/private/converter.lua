local converter = {}

function converter.to_table_from_array(userdata)
    if userdata == nil then return {} end
    local lua_table = {}
    
    if userdata.Length then
        for i = 0, userdata.Length - 1 do
            table.insert(lua_table, userdata[i])
        end
    end

    return lua_table
end

function converter.table_to_dotnet_dict(table)
    import("System")
    
    local dict_type = Type.GetType("System.Collections.Generic.Dictionary`2[[System.String],[System.Object]]");
    local dict = Activator.CreateInstance(dict_type)
    
    for k, v in pairs(table) do
        dict:Add(tostring(k), v)
    end 
    
    return dict
end

function converter.dotnet_dict_to_table(dict)
    local res = {}

    local enum = dict:GetEnumerator()
    while enum:MoveNext() do
        local key_value_pair = enum.Current
        local key = key_value_pair.Key
        local value = key_value_pair.value

        res[key] = value
    end

    if enum.Dispose ~= nil then
        enum:Dispose()
    end
    
    return res;
end

return converter;