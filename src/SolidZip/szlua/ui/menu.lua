local menu = {};

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

---Creates a menu item to add it into application menu
---@return table menu_item
---menu_item configuration:
---1) onclick(args) method must be created to handle click at the menu_item in application ui
---2) icon it's flied with menu_item icon, must be configured from media.icon module
---3) title (string) title what will be represent  
function menu.ctor_element()
    return {
        build = function(self) 
            local menu_item = MenuItem();
            if type(self.onclick) == "function" then
                menu_item.Click:add(function (_,  args) 
                    self.onclick(args);
                end)
            end
            assert(self.title == nil or type(self.title) == "string")
            menu_item.Header = self.title;
            menu_item.Icon = self.icon
            self._wpf_menu_item.Items:Add(menu_item)
        end;
        _wpf_menu_item = MenuItem();
    }
end

return menu;