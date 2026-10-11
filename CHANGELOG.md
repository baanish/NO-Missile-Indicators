# Changelog

## 0.2.0 - 2026-10-11

### Changed

- The arrows sit on a ring around the HUD instead of the screen edge, so they're near where you're looking. `Missile Arrows → RingSize` sets its radius in percent of the screen height (default 30); 200 puts them back on the screen edge.

### Added

- The arrows flash between `Missile Arrows → Color` (red) and the new `FlashColor` (yellow) about three times a second, as the game's missile warning flashes, so they read as urgent.
- `Missile Arrows → Style`: `ProjectWingman` shows every warned missile as Project Wingman's 3D indicator: a solid triangular plate with a yellow block at each corner, on a sphere just ahead of the camera seen from a little above, at the point toward the missile and turned to face it. Its side toward the missile is red (`Color`) and its back yellow (`FlashColor`), so a missile behind you shows a red plate below the middle, one ahead a yellow one above it, and one beside you a plate at that side.

## 0.1.0 - 2026-09-30

First release: the missile arrows from Baanish UI Improvements 0.4.0, as a mod of their own.

### Added

- A red arrow on the screen edge toward each incoming missile outside the view, on the correct side even when the missile is behind you. Only missiles the game's missile warning knows about get one, and each arrow points where the game's HUD marker for that missile is, frozen and faded to half once the game marks it outdated.
- The arrows are off in player-versus-player games, where another human isn't on your faction (including one who hasn't picked a side yet), unless `Missile Arrows → AllowInPvp` is on. F1 shows whether the arrows are showing right now, and why.
- A master switch, `General → Enabled`, that hides the arrows without uninstalling.
- On first load, the arrow settings are copied from Baanish UI Improvements' settings file, which is only read.
