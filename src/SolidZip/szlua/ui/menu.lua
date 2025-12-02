local menu = {};

if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

---Creates a menu item to add it into application menu
---@return table menu_item
---menu_item elements:
---1) onclick(args) method must be created to handle click at the menu_item in application ui
---2) icon it's flied with menu_item icon, must be configured from media.icon module
---3) title (string) title what will be represent  
---4) override_styles (boolean) (default: true) marks that on building MenuItem needs to use default application style (SzMenuItem) 
---5) _wpf_menu_item WPF MenuItem 
---6) build (function) method for creating WPF MenuItem from lua-table 
function menu.ctor_element()
    local dispatcher = require("szlua\\ui\\dispatcher");
    local res = require("szlua\\ui\\resources");
    local element =  {
        build = function(self)
            dispatcher.exec(function()
                
                if override_styles then
                    self._wpf_menu_item.Style = res.SzMenuItem;
                end
                assert(self.title == nil or type(self.title) == "string", "title must be string")
                self._wpf_menu_item.Header = self.title
                self._wpf_menu_item.Icon = self.icon
                
                if type(self.onclick) == "function" then
                    self._wpf_menu_item.Click:Add(function(sender, args)
                        self.onclick(args);
                    end)
                end

                return self._wpf_menu_item
            end)
        end;
        override_styles = true;
    }
    dispatcher.exec(function() 
        element._wpf_menu_item = MenuItem()
    end)
    return element;
end

return menu;