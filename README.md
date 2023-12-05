# RouteRandom

Route Random adds a couple commands for routing to a random planet

`route random` will get a random planet

`route randomfilterweather` will get a random planet following the disallowed weather conditions in the config

Config options for:

- Allowing each weather type to be chosen by `route randomfilterweather`
- Allowing costly planets to bo be chosen
- Removing the cost of costly planets when they're randomly routed to
- Skipping the confirmation screen
- Preventing the currently orbited planet from being randomly routed to
- Completely hiding the planet randomly routed to, in both the terminal and helm screens. NOTE: Will still hide planet info when chosen purposefully

Changelog:

- v1.0.0
  - Released
- v1.0.1
  - Tweaked description
- v1.1.0
  - Add configs for costly planets
  - Add config to allow or prevent Mild weather being chosen
  - Add description of `random` and `randomwithweather` commands to `moons` command
  - Fix issues when no moon was suitable for `route random`
- v1.2.0
  - Switch logic, `route random` now chooses a random planet while `route randomfilterweather` will filter out planets with disallowed weather conditions
  - Add config to skip confirmation screen
  - Add config to prevent choosing current planet
  - Add config to hide planet
  - Fix Experimentation not obeying weather filters
  - Fix `route random` and `route randomwithweather` commands sometimes not working
- v1.2.1
  - Fix an issue with skipping confirmation and removing cost of costly planets, but not hiding planet info
  - Fix a typo in logged awaken message
- v1.2.2
  - Prevent Company Building from being hidden
  - Fix video clips still sometimes playing when hiding orbited moon
  - Fix Dine showing up twice as often as other moons
