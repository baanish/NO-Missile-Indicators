# How it works

The plugin draws on top of the game and never changes its state. Nothing in the game is patched. Every frame it reads the game's HUD and missile warning, works out where each arrow goes, and moves a few UI images it owns. If a game update breaks something it calls, the plugin logs the error, disables itself, and leaves the game running untouched. If one removes the HUD arrow it copies by reflection, it logs a warning at startup and draws nothing.

## One frame

```text
Plugin.LateUpdate   (every frame, inside Guard)
  ├─ General.Enabled off, or PvpGate.HidesArrows   remove the arrows and stop
  └─ IncomingMissileArrows.Render
       for each missile in MissileWarning.knownMissiles
         ├─ MissileTrack         where the game's HUD marker for it sits, and whether that's live
         ├─ KnownPositions       hold the last known position once the marker goes outdated
         ├─ ScreenEdge.OnScreen  skip a missile that's on screen
         └─ ScreenEdge.Pin       the point on the ring around the screen centre
```

`Guard` catches any exception, logs it, disables the plugin, and removes the arrows, so an error doesn't repeat every frame. Turning **General → Enabled** off removes the arrows the same frame, and they rebuild themselves on the next render once it's back on.

## Placing an arrow

`IncomingMissileArrows` draws one arrow per missile in the aircraft's `MissileWarning.knownMissiles`, the list the game's missile warning fills. Each arrow is a copy of `CombatHUD`'s private `targetArrow`, read by reflection, so it shares the HUD canvas, sprite, and visibility. Its children are destroyed, since any label among them would show the target's name. Every arrow flashes with the game's own formula for a locked missile's notch indicator (`ThreatItem.AnimateItem`): `Color.Lerp(yellow, red, sin(timeSinceLevelLoad × 20) + 0.5)`, with **Flash colour** and **Arrow colour** in place of yellow and red.

In the ProjectWingman style, `WingmanSphere` builds Project Wingman's 3D indicator instead and one `WingmanSphereGraphic`, parented beside the target arrow, draws every missile's plate as a single mesh. The sphere is centred 1 unit ahead of the camera with radius 0.25, scaled so its edge sits at **Ring size** on screen, and viewed from 20 degrees above, as Project Wingman's chase camera sees its sphere: a missile behind lands below the middle and one ahead above it, so the two never stack on the flight path marker. Each plate is a solid triangular prism on the sphere toward the missile, red in front and yellow behind, turned half way from the missile's direction toward the camera so a plate beside you reads instead of showing its edge; a yellow cuboid stands out of its back at each corner, square to the plate. `WingmanSphere` keeps only faces turned toward the camera and shades each from a light above and in front; the graphic draws them far to near, with a soft glow, a denser rim and a light outline on the plate's big triangle. A plate is about 60 vertices; the graphic reads its transform and the screen size once per mesh rather than per vertex, since each Unity call from a plugin costs more than the math around it. Changing the style destroys the indicators and builds the other kind on the next frame.

The arrow points where the game's flashing HUD marker for that missile sits (`HUDUnitMarker.UpdatePosition`): at the missile while the marker is current, and at `FactionHQ.TryGetKnownPosition` once the game marks it `outdated`. `KnownPositions` holds that position so the arrow stops moving, and the arrow fades to half alpha, as the marker does. A missile the HUD has no marker for gets no arrow.

`ScreenEdge` takes the missile's position in camera space (X right, Y up, Z forward). A point that `OnScreen` finds in front of the camera and projecting inside the screen gets no arrow. `Pin` puts the arrow on the ray from the screen centre toward the point's X and Y, at **Ring size** percent of the screen height or at the screen edge, whichever is nearer, rotated to point along the ray. A missile dead astern gets the bottom.

This is custom math on purpose. `Camera.WorldToScreenPoint` mirrors points behind the camera, so an arrow built from it, like the game's own target arrow, points away from a missile closing from behind. Working in camera space puts that arrow on the side the missile is on. `ScreenEdge` and `KnownPositions` are Unity-free and unit-tested.

A new scene brings a new `CombatHUD`, and destroying the old one takes the arrows with it. `IncomingMissileArrows` notices that the HUD changed, or that its first arrow is gone, and starts over: it forgets the stored positions, since a new mission reuses unit ids, and copies the arrow again.

## What the mod may show

The arrows tell the player nothing the game hasn't. In `Assembly-CSharp`, once `MissileWarning` moves a missile into `knownMissiles`, `ThreatList` flashes its HUD marker (`CombatHUD.FlashMarker`) and its map icon (`DynamicMap.FlagIncomingMissile`). The marker shows the missile where it is while the player's side tracks it. After 4 s without a spot, the game switches the marker to its outdated sprite at half opacity, pinned to `FactionHQ.TryGetKnownPosition` (`HUDUnitMarker.SetOutdated`).

| Shows | Reads | Fair because |
| --- | --- | --- |
| An arrow per warned missile | `MissileWarning.knownMissiles` of `CombatHUD.aircraft` | The same list that drives the game's warning |
| A ProjectWingman plate on screen | The same position, as a direction from the camera | The game's marker already shows that missile on screen; the plate's side and size come from that direction alone |
| Where the arrow points | The missile's `HUDUnitMarker` through `CombatHUD.TryGetMarker`: the missile while current, `TryGetKnownPosition` once `outdated` | Points where the game's own marker is, faded when the marker is |

## Off in PvP

Against another player, an arrow toward every missile the warning knows may be a competitive advantage, so the arrows stay off in player-versus-player games unless the player allows them. `PvpRule.Judge` decides from three inputs, and `PvpGate` reads them from the game once a second:

- **Multiplayer**: `GameManager.gameState == GameState.Multiplayer`, the state a client sets on connecting and a host starts a multiplayer server in (`NetworkManagerNuclearOption`). The game's own `NetworkManagerNuclearOption.HasOtherPlayers` makes the same test. Single player and the mission editor have no other players.
- **Your faction**: `Player.HQ` of the local player (`GameManager.GetLocalPlayer`), null until you pick one.
- **Everyone else's faction**: `Player.HQ` of each other entry in `UnitRegistry.playerLookup`, which every `Player` joins as its network object starts on this client. `Player` is a human connection. AI aircraft have none.

Any faction other than yours counts as hostile, as it does for the game when `FactionHQ.DeployAIAircraft` counts enemy players. A player who hasn't picked a faction yet counts too: still joining or watching, they may pick the other side. The verdict is PvP when any other player isn't on your faction, and unknown when the local player or its faction can't be read. Both hide the arrows unless **Allow in PvP** is on. Once a second is plenty for players joining, leaving, or picking a faction, and it saves walking the player list every frame. The gate logs one line when the arrows go and one when they come back. `PvpRule` is Unity-free and unit-tested.

## Settings

`ModSettings` binds every setting through its `Bind` helper, which tags each one with a `ConfigurationManagerAttributes`: the name F1 shows and an `Order`. ConfigurationManager lists a plugin's sections in the order they're first bound and sorts settings by descending `Order`, so the helper counts `Order` down and the constructor reads top to bottom like the F1 window.

On first load, `Plugin` copies the arrow settings from Baanish UI Improvements, which shipped the arrows up to 0.4.0. BepInEx writes the plugin's settings file as the first setting binds, so `Awake` reads the file before binding. If it held no value, `OldSettingsImport.ValuesToCopy` reads `com.baanish.nuclearoption.uiimprovements.cfg`, takes the values under `[HUD / Missile Arrows]` or `[Missile Arrows]`, and `Plugin` sets each matching setting with `ConfigEntryBase.SetSerializedValue`. The old file is only read. `OldSettingsImport` is Unity-free and tested against a real 0.4.0 settings file.
