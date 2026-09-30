# User guide

The [install guide](INSTALL.md) covers installing, updating, and uninstalling.

## What you see

When a missile is incoming and outside your view, a red arrow on the screen edge points the way to turn toward it, so you can find it and shoot it down. Each missile gets its own arrow. A missile behind you gets an arrow on the side it's on, so a missile behind and to your left gets an arrow on the left edge. Once the missile is on screen, its arrow goes away.

Only missiles the game's missile warning already knows about get an arrow, and each arrow points where the game's flashing HUD marker for that missile is, so the arrows show nothing the game hasn't told you. When the game marks a missile's position as outdated, its arrow stays pointing at the last known position and fades to half, as the game's marker does. The arrow is a copy of the game's own off-screen target arrow in a different colour, so it hides when the HUD does.

## Off in PvP by default

The arrows are off in player-versus-player games unless you turn on **Allow in PvP**. Against another player, an arrow toward every incoming missile may be a competitive advantage, so only turn it on when the other players know and agree.

A game counts as PvP when it's multiplayer and at least one other human player is on a faction other than yours. The mod also keeps the arrows off in a multiplayer game while it can't tell, such as before you've picked a faction. It checks once a second, so the arrows go or come back within a second of a player joining, leaving, or picking a faction.

| Game | Arrows |
| --- | --- |
| Single player or the mission editor | Shown |
| Multiplayer, co-op against AI | Shown |
| Multiplayer, other humans only on your side | Shown |
| Multiplayer, another human on a hostile faction | Off, unless **Allow in PvP** is on |
| Multiplayer, before you pick a faction | Off, unless **Allow in PvP** is on |

In F1, the **Allow in PvP** row also says what the arrows are doing right now and why, such as `Now: shown, not a PvP game` or `Now: hidden, a player is on a hostile side`.

`BepInEx/LogOutput.log` gets one line when the arrows go and one when they come back, such as:

```text
[Info   :NO Missile Indicators] Missile arrows hidden: another player is on a hostile faction. Missile Arrows > Allow in PvP overrides this.
```

## Settings

Change settings in game with F1 if Configuration Manager is installed. Changes apply immediately. Click **NO Missile Indicators** to open its settings, and hover over a setting's name for a longer description.

Without Configuration Manager, edit `BepInEx/config/com.baanish.nuclearoption.missileindicators.cfg` while the game is closed. Each section below is a `[Section]` heading in the file, and the key is the name before `=`. Colours are hex `RRGGBBAA`.

The table lists the settings in the order F1 shows them.

| Section | F1 name | Key | Default | What it does |
| --- | --- | --- | --- | --- |
| General | Enabled | Enabled | `true` | Off hides everything the mod draws, as if it were not installed. Use it to check whether a problem comes from the mod or the game. |
| Missile Arrows | Show missile arrows | ShowArrows | `true` | Point an arrow at each incoming missile outside the view. |
| Missile Arrows | Allow in PvP | AllowInPvp | `false` | Also show the arrows in player-versus-player games. There they may be a competitive advantage, so only turn this on when the other players know and agree. |
| Missile Arrows | Arrow colour | Color | `FF4033FF` | Arrow colour. |

### Settings from Baanish UI Improvements

The arrows used to ship inside Baanish UI Improvements, up to 0.4.0. The first time this mod loads, while it has no settings file of its own, it copies **Show missile arrows** and **Arrow colour** from that mod's `BepInEx/config/com.baanish.nuclearoption.uiimprovements.cfg`. It reads them from the old `[Missile Arrows]` section, or from `[HUD / Missile Arrows]` where a 0.5.0 build saved them. It never writes the old file. **Allow in PvP** is new and starts off.
