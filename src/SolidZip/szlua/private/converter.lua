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

return converter