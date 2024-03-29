- v1.3.1: Fixed an issue that broke the terminal when using either command with a specific config setup
- v1.3.0: Generified some code to allow modded moon support
  - Assumes that modded moons are registered the same way as regular ones (Every moon I have tested with has been thanks to the APIs they use taking this into consideration)
    - If a modded moon doesn't do this for some reason, it won't be randomly chosen by this mod
    - This limitation is sadly unavoidable
- v1.2.2: Fixes
  - Prevent Company Building from being hidden
  - Fix video clips still sometimes playing when hiding orbited moon
  - Fix Dine showing up twice as often as other moons
- v1.2.1: Fixes
  - Fix an issue with skipping confirmation and removing cost of costly planets, but not hiding planet info
  - Fix a typo in logged awaken message
- v1.2.0: Switch `randomfilterweather` and `random` commands around
  - Switch logic, `route random` now chooses a random planet while `route randomfilterweather` will filter out planets with disallowed weather conditions
  - Add config to skip confirmation screen
  - Add config to prevent choosing current planet
  - Add config to hide planet
  - Fix Experimentation not obeying weather filters
  - Fix `route random` and `route randomwithweather` commands sometimes not working
- v1.1.0: Added some new configs
  - Add configs for costly planets
  - Add config to allow or prevent Mild weather being chosen
  - Add description of `random` and `randomwithweather` commands to `moons` command
  - Fix issues when no moon was suitable for `route random`
- v1.0.1: Tweaked description
- v1.0.0: Initial release
