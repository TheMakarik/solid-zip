local events = require("szlua\\events");

--_G.event_name and _G.event_args must be loaded from test
events.raise(_G.event_name, _G.event_args)