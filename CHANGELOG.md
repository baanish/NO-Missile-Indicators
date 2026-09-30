# Changelog

## 0.1.0 - Unreleased

First release: the missile arrows from Baanish UI Improvements 0.4.0, as a mod of their own.

### Added

- A red arrow on the screen edge toward each incoming missile outside the view, on the correct side even when the missile is behind you. Only missiles the game's missile warning knows about get one, and each arrow points where the game's HUD marker for that missile is, frozen and faded to half once the game marks it outdated.
- The arrows are off in player-versus-player games, where another human is on a hostile faction, unless `Missile Arrows → AllowInPvp` is on.
- A master switch, `General → Enabled`, that hides the arrows without uninstalling.
- On first load, the arrow settings are copied from Baanish UI Improvements' settings file, which is only read.
