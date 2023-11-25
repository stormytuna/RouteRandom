# RouteRandom

Route Random adds a couple commands for routing to a random planet.

`route random` will get a random planet following the allowed weathers in the config

`route randomwithweather` will ignore the config and allow all weather types

Config options for:

- Allowing each weather type to be chosen by `route random`
- Allowing costly planets to bo be chosen
- Removing the cost of costly planets when they're randomly chosen

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
