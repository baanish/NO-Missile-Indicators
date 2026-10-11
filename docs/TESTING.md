# Testing

This page separates automated checks from in-game observations. A screenshot from one flight shows the tested setup; it doesn't prove every map or aircraft.

## Automated

```powershell
dotnet run --project tests/NoMissileIndicators.Tests -c Release
```

30 tests pass. They cover:

- indicator placement (`ScreenEdge`): which missiles are on screen (never one behind), the matching side of the ring off screen, the correct side for a missile behind you, above and behind, or dead astern, and a ring larger than the screen stopping at its edge;
- the Project Wingman plate (`WingmanSphere`): a missile behind shows the red front and hides the yellow back, large, below the middle; one ahead the yellow back, smaller, above the middle; one left or right a slanted plate on that side, with its corner bars running in; and a missile straight above;
- tracked positions (`KnownPositions`): live, stale, first seen stale, and never known;
- copying the old settings (`OldSettingsImport`): a real Baanish UI Improvements 0.4.0 settings file, the 0.5.0 section layout, which layout wins, a saved value of this mod's own stopping the copy, and no old file;
- the PvP rule (`PvpRule`): single player, co-op against AI, humans only on your side, one hostile human, a faction change, a player with no faction yet counting as PvP, and an unreadable faction or player list.

The suite doesn't start Unity or the game. It doesn't cover drawing, reading the game's missile warning, HUD markers, or player list, or game-version compatibility.

## In-game status for v0.1.0

NO Missile Indicators itself hasn't been checked in game yet. The arrow code is the code Baanish UI Improvements shipped in 0.4.0 and carried through its 0.5.0 work, so the rows marked "in Baanish UI Improvements" were observed there. That was on Nuclear Option 0.34.2 (Steam build 24724372), Unity 2022.3.62, BepInEx 5.4.23.4, in single-player free flight and the Escalation mission, with other client mods loaded, on 2026-09-30.

| Area | Status | Evidence or remaining work |
| --- | --- | --- |
| Missile arrows | Partly observed in Baanish UI Improvements | An arrow points at an incoming missile off screen and follows the game's own HUD marker for it. Getting shot down once disabled the mod, a bug since fixed. A missile behind you and two missiles at once weren't recorded as separate checks. |
| Multiplayer | Observed in Baanish UI Improvements 0.4.0 | A multiplayer session showed no problems. Whether a missile came in during it wasn't recorded. |
| Plugin loads | Observed | `NO Missile Indicators 0.1.0 loaded.` in `LogOutput.log`. |
| Alongside NO Map Overhaul | Observed | Both load, and each missile gets one arrow. |
| Settings copied from the old mod | Observed | With `com.baanish.nuclearoption.uiimprovements.cfg` in place and no settings file of this mod's own, the log said `Copied 2 missile arrow settings`, and F1 showed the old values. |
| Master switch | Not yet checked | Turning General → Enabled off in F1 removes the arrows, and turning it back on restores them. |
| Off in PvP | Partly observed | In a multiplayer match with no other players, the arrows show, the log says `Missile arrows shown: no other player is on a hostile faction.`, and F1's Allow in PvP row reads `Now: shown, not a PvP game`. A match with a human on the other faction, where the arrows should hide, isn't checked yet. |

## In-game status for the ring and the flashing arrows

Automated in-game check by an agent through NO-Agentic-Framework, not yet a person's flight: Nuclear Option 0.34.2 (Steam build 24724372), BepInEx 5.4.23.4, the branch build swapped into the installed mod stack, single-player `airborne` mission in a Multirole1 at 1920x1080 in the cockpit view, four AAM2s spawned around the jet while it turned, on 2026-10-11. A before and after video from the same setup compares v0.1.0.

| Area | Status | Evidence or remaining work |
| --- | --- | --- |
| Arrows on the ring | Observed by an agent | With Ring size 30, arrows sit about 325 px from the screen centre, clear of the pitch ladder, and point toward each warned missile off screen. Ring size 40 put them on the canopy frame above and over the radar below. |
| Flashing | Observed by an agent | Arrows alternate between red and yellow about three times a second, in step with the game's missile warning. |
| ProjectWingman style | Observed by an agent | One missile at a time from behind, ahead, right, left, and two o'clock gives a shaded red prism below the middle, a small yellow one above it, yellow plates turned to show a red outer edge at the right and left, and a yellow plate up and right; corner blocks show on every plate. |
| Per-frame cost | Measured by an agent | A temporary timing build, six warned missiles at once at 1920x1080: Arrows 13 µs a frame; ProjectWingman 86 µs placing the plates plus 70 µs meshing them, about 0.16 ms. With no missile warned, both styles cost about 4 µs. Meshing first cost 400 µs, cut by dropping a dashed inner line and edge lines too small to see and by reading Unity's transform once per mesh. |
| Third-person view, other resolutions | Not yet checked | The ring is a fraction of the screen height and centred on the screen. |

"Observed" means seen in game during manual testing. It doesn't mean an automated test covers it.
