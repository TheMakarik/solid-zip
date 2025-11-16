local events = require("szlua\\events");

--_G.event_name must be loaded from test
events.raise(_G.event_name)