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

function icons.from_file(path)
    local extractor = _sz("AssociatedIconExtractor")
    local icon = extractor:Extract(path)

    return to_bitmap_image(icon);
end

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