if(_G.import ~= nil) then
    import('System')
    import('WindowsBase')
    import('PresentationCore')
    import('PresentationFramework')
    import('System.Windows')
    import('System.Windows.Interop')
    import('System.Windows.Media.Imaging')
    import("Material.Icons.WPF")
    import("Material.Icons")
end

local icons = {}

function icons.from_file(path)
    local extractor = _sz("AssociatedIconExtractor")
    local icon = extractor:Extract(path)

    return to_bitmap_image(icon);
end

function icons.from_image(path)
    local path = "pack://application:,,," .. path;
    local bitmap = BitmapImage();
    bitmap:BeginInit();
    bitmap.UriSource = Uri(path, UriKind.Absolute);
    bitmap:EndInit();
    bitmap:Freeze();
    return bitmap;
end

function icons.app_icon()
    local path_collection = _sz("PathCollection")
    local path = "pack://application:,,," .. path_collection.IconPath
    local bitmap = BitmapImage();
    bitmap:BeginInit();
    bitmap.UriSource = Uri(path, UriKind.Absolute);
    bitmap:EndInit();
    bitmap:Freeze();
    return bitmap;
end

function icons.from_extension(extension)
    local extractor = _sz("ExtensionIconExtractor")
    local icon = extractor:Extract(extension)

    return to_bitmap_image(icon);
end


function icons.from_material(name, h, w)
    local dispatcher = require("szlua\\ui\\dispatcher")
    local icon = nil
    
    dispatcher.exec(function()
        icon = load_icon(name);

        if h ~= nil then
            icon.Height = h
        end

        if w ~= nil then
            icon.Width = w
        end
    end)

    return icon
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