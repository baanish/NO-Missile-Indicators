NO Missile Indicators v@VERSION@

A red arrow on the screen edge toward each incoming missile outside your
view, pointing the way to turn, so you can find it and shoot it down. A
missile behind you gets its arrow on the side it's on.

Only missiles the game's missile warning already knows about get an arrow,
and each arrow points where the game's own HUD marker for that missile is.
When the game marks a missile's position as outdated, the arrow stays at the
last known position and fades to half, as the marker does.

The arrows are off in player-versus-player games, where another human
player is on a hostile faction. There they may be a competitive advantage.
Missile Arrows > Allow in PvP turns them on; only do that when the other
players know and agree. Single player, co-op against AI, and multiplayer
with humans only on your side show the arrows.

The mod only draws on your screen. It doesn't patch the game and sends
nothing over the network. Switch off General > Enabled to hide the arrows
without uninstalling.

The arrows used to ship inside Baanish UI Improvements, up to 0.4.0. If you
still run that version, update it to NO Map Overhaul 0.5.0 or later, which
no longer draws them, or you'll see two arrows per missile. On first load
this mod copies your arrow settings from the old mod's settings file.

Requirements

- Nuclear Option 0.34.2
- BepInEx 5, tested with 5.4.23.4
- BepInEx Configuration Manager, optional, to change settings with F1

Install

The plugin-only archive contains the BepInEx folder tree. Extract it beside
NuclearOption.exe.

For a manual install from the flat NOMM archive, copy
NoMissileIndicators.dll to:

  BepInEx/plugins/NoMissileIndicators/NoMissileIndicators.dll

Start the game once. BepInEx creates the settings file:

  BepInEx/config/com.baanish.nuclearoption.missileindicators.cfg

Step-by-step install guide:
https://github.com/baanish/NO-Missile-Indicators/blob/main/docs/INSTALL.md

Settings:
https://github.com/baanish/NO-Missile-Indicators/blob/main/docs/USER_GUIDE.md

Source and full documentation:
https://github.com/baanish/NO-Missile-Indicators

MIT license. See LICENSE.txt.
