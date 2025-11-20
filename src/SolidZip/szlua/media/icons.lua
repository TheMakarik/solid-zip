import("System.Windows.Interop")

local icons = {}

---Extract icon from specific path using WinAPI
---@param path string to extract
---@return userdata WPF ImageSource instance 
function icon.from_file(path)
    local extractor = _sz("AssociatedIconExtractor")
    local icon = extractor:Extract(path)
    
    icon:Dispose()
end

return icons