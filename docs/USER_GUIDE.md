# User guide

The [install guide](INSTALL.md) covers installing, updating, and uninstalling.

## What you see

When a missile is incoming and outside your view, a red arrow on a ring around the HUD points the way to turn toward it, so you can find it and shoot it down. Each missile gets its own arrow. A missile behind you gets an arrow on the side it's on, so a missile behind and to your left gets an arrow on the left of the ring. Once the missile is on screen, its arrow goes away. **Ring size** sets how far from the screen centre the arrows sit; the arrows never leave the screen, so a large value puts them on the screen edge. The arrows flash between red and yellow, as the game's missile warning does.

With **Style** set to `ProjectWingman`, each warned missile gets Project Wingman's 3D indicator instead, and keeps it while it's on screen. Picture a sphere around you, the size of the ring, seen from a little above, with a solid triangular plate on it at the point toward the missile, turned to face the missile. The side facing the missile is red and the back is yellow, and a small yellow block stands out of the back at each corner, into the sphere. You see the red side only when a missile is well behind you, from about half past four round to half past seven, as a red plate below the middle of the screen. Anywhere else you see the yellow back: a missile ahead shows a smaller yellow plate above the middle, one to your side a yellow plate at that side with its red edge facing out, and one behind your wing a yellow plate below the middle. So the plate's position says where the missile is, and red means it's well behind you.

Only missiles the game's missile warning already knows about get an arrow, and each arrow points where the game's flashing HUD marker for that missile is, so the arrows show nothing the game hasn't told you. When the game marks a missile's position as outdated, its arrow stays pointing at the last known position and fades to half, as the game's marker does. The arrow is a copy of the game's own off-screen target arrow in a different colour, so it hides when the HUD does.

## Off in PvP by default

The arrows are off in player-versus-player games unless you turn on **Allow in PvP**. Against another player, an arrow toward every incoming missile may be a competitive advantage, so only turn it on when the other players know and agree.

A game counts as PvP when it's multiplayer and at least one other human player isn't on your faction. That includes a player still joining, or watching before picking a side, since they may pick the other one. The mod also keeps the arrows off in a multiplayer game while it can't tell, such as before you've picked a faction. It checks once a second, so the arrows go or come back within a second of a player joining, leaving, or picking a faction.

| Game | Arrows |
| --- | --- |
| Single player or the mission editor | Shown |
| Multiplayer, co-op against AI | Shown |
| Multiplayer, other humans only on your side | Shown |
| Multiplayer, another human on a hostile faction | Off, unless **Allow in PvP** is on |
| Multiplayer, another human who hasn't picked a side yet | Off, unless **Allow in PvP** is on |
| Multiplayer, before you pick a faction | Off, unless **Allow in PvP** is on |

In F1, the **Allow in PvP** row also says what the arrows are doing right now and why, such as `Now: shown, not a PvP game` or `Now: hidden, a player isn't on your side`.

`BepInEx/LogOutput.log` gets one line when the arrows go and one when they come back, such as:

```text
[Info   :NO Missile Indicators] Missile arrows hidden: another player isn't on your faction. Missile Arrows > Allow in PvP overrides this.
```

## Settings

Change settings in game with F1 if Configuration Manager is installed. Changes apply immediately. Click **NO Missile Indicators** to open its settings, and hover over a setting's name for a longer description.

Without Configuration Manager, edit `BepInEx/config/com.baanish.nuclearoption.missileindicators.cfg` while the game is closed. Each section below is a `[Section]` heading in the file, and the key is the name before `=`. Colours are hex `RRGGBBAA`.

The table lists the settings in the order F1 shows them.

| Section | F1 name | Key | Default | What it does |
| --- | --- | --- | --- | --- |
| General | Enabled | Enabled | `true` | Off hides everything the mod draws, as if it were not installed. Use it to check whether a problem comes from the mod or the game. |
| Missile Arrows | Show missile arrows | ShowArrows | `true` | Point an arrow at each incoming missile outside the view. |
| Missile Arrows | Style | Style | `Arrows` | `Arrows`: a flashing arrow toward each missile outside the view. `ProjectWingman`: a 3D triangle toward every warned missile, red when it's well behind you and yellow elsewhere, below the middle of the screen for a missile behind you. |
| Missile Arrows | Ring size | RingSize | `30` | Radius of the ring the arrows sit on, or of the `ProjectWingman` sphere, in percent of the screen height, from 10 to 200. 200 puts arrows on the screen edge, even on a 32:9 screen. The sphere stops short of the screen's top and bottom. |
| Missile Arrows | Allow in PvP | AllowInPvp | `false` | Also show the arrows in player-versus-player games. There they may be a competitive advantage, so only turn this on when the other players know and agree. |
| Missile Arrows | Arrow colour | Color | `FF4033FF` | Colour the arrows flash from, and of the side of a `ProjectWingman` plate facing the missile. |
| Missile Arrows | Flash colour | FlashColor | `FFEB04FF` | Colour the arrows flash to, and of the back and corner blocks of a `ProjectWingman` plate. |

### Settings from Baanish UI Improvements

The arrows used to ship inside Baanish UI Improvements, up to 0.4.0. The first time this mod loads, while it has no settings file of its own, it copies **Show missile arrows** and **Arrow colour** from that mod's `BepInEx/config/com.baanish.nuclearoption.uiimprovements.cfg`. It reads them from the old `[Missile Arrows]` section, or from `[HUD / Missile Arrows]` where a 0.5.0 build saved them. It never writes the old file. **Allow in PvP** is new and starts off.
