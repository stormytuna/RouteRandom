# RouteRandom

Route Random adds a couple commands for routing to a random planet. Supports (almost all) modded moons!

`route random` will get a random planet

`route randomfilterweather` will get a random planet following the disallowed weather conditions in the config

Config options for:

- Allowing each weather type to be chosen by `route randomfilterweather`
- Allowing costly planets to bo be chosen
- Removing the cost of costly planets when they're randomly routed to
- Skipping the confirmation screen
- Preventing the currently orbited planet from being randomly routed to
- Completely hiding the planet randomly routed to, in both the terminal and helm screens. NOTE: Will still hide planet info when chosen purposefully
