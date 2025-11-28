if(_G.import ~= nil) then
    import('System')
    import('WindowsBase')
    import('PresentationCore')
    import('PresentationFramework')
    import('System.Windows')
    import('System.Windows.Interop')
    import('System.Windows.Media.Imaging')
end

local icons = {}

---Extracts an icon from a file path and converts it to BitmapImage
---@param path string The file system path to extract icon from
---@return table # BitmapImage object or empty BitmapImage if extraction fails
function icons.from_file(path)
    local extractor = _sz("AssociatedIconExtractor")
    local icon = extractor:Extract(path)

    return to_bitmap_image(icon);
end

---Extracts an icon from a file extension and converts it to BitmapImage
---@param extension string The file extension (e.g., ".txt", ".exe") to extract icon for
---@return table # BitmapImage object or empty BitmapImage if extraction fails
function icons.from_extension(extension)
    local extractor = _sz("ExtensionIconExtractor")
    local icon = extractor:Extract(extension)

    return to_bitmap_image(icon);
end

function to_bitmap_image(icon)
    if icon == nil or icon.HIcon == nil or icon.HIcon:ToInt32() == 0 then
        local empty_bitmap = BitmapImage()
        return empty_bitmap
    end

    local success, result = pcall(function()
        return Imaging.CreateBitmapSourceFromHIcon(
                icon.HIcon,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
        )
    end)
    
    pcall(function()
        if icon.Dispose then
            icon:Dispose()
        end
    end)

    if success then
        return result
    else
        return BitmapImage()
    end
end

return icons