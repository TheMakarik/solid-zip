local menu = {};


if(_G.import ~= nil) then
    import ('System', 'System.Windows')
    import ('PresentationFramework', 'System.Windows')
    import ('System.Windows.Controls')
end

---Creates a menu item to add it into application menu
---@return table menu_item
---menu_item elements:
---1) onclick_event szlua event that occurs at menu item click
---2) icon it's flied with menu_item icon, must be configured from media.icon module
---3) title (string) title what will be represent
---5) _wpf_menu_item WPF MenuItem 
---6) build (function) method for creating WPF MenuItem from lua-table 
---7) submenu_close_event szlua event that occurs than submenu was closed 
---8) submenu_opened_event szlua event that occurs than submenu was opened
function menu.ctor_element()
    local dispatcher = require("szlua\\ui\\dispatcher");
    local redirector = require("szlua\\events\\event_redirector");
    local element =  {
        build = function(self)
            dispatcher.exec(function()
                
                assert(self.title == nil or type(self.title) == "string", "title must be string")
                self._wpf_menu_item.Header = self.title
                self._wpf_menu_item.Icon = self.icon
                
                if type(self.onclick_event) == "string" then
                    redirector.redirect(self._wpf_menu_item, "Click", self.onclick_event);
                end
                if type(self.submenu_close_event) == "string" then
                    redirector.redirect(self._wpf_menu_item, "SubmenuClosed", self.submenu_close_event);
                end
                if type(self.onclick_event) == "string" then
                    redirector.redirect(self._wpf_menu_item, "Click", self.onclick_event);
                end
            end)
            return self._wpf_menu_item
        end;
    }
    dispatcher.exec(function() 
        element._wpf_menu_item = MenuItem()
    end)
    return element;
end

return menu;