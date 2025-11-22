if(_G.import ~= nil) then
    import('System', 'System.Windows')
    import('PresentationFramework', 'System.Windows')
    import("System.Windows.Interop")
    import('System.Windows.Media.Imaging')
end


local icons = {}

---Extract icon from specific path using WinAPI
---@param path string to extract
---@return userdata WPF ImageSource instance 
function icon.from_file(path)
    local extractor = _sz("AssociatedIconExtractor")
    local icon = extractor:Extract(path)
   
    return to_bitmap_image(icon);
  
end

---Extract associated with extension icon
---@param extension string file extension to get icon(note that you can use path, and it will work)
---@return userdata WPF ImageSource instance 
function icon.from_extension(extension)
    local extractor = _sz("ExtensionIconExtractor")
    local icon = extractor:Extract(extension) --you can use path, of course
    
    return to_bitmap_image(icon);
end

function to_bitmap_image(icon)
    if icon.HIcon == 0 then
        return BitmapImage();
    end

    local result = Imaging.CreateBitmapSourceFromHIcon(
            icon.HIcon,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions()
    )
    
    icon:Dispose()
    return result;
end

return icons