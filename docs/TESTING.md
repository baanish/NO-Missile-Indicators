# Testing

This page separates automated checks from in-game observations. A screenshot from one flight shows the tested setup; it doesn't prove every map or aircraft.

## Automated

```powershell
dotnet run --project tests/NoMissileIndicators.Tests -c Release
```

22 tests pass. They cover:

- arrow placement (`ScreenEdge`): none on screen, the matching edge off screen, and the correct side for a missile behind you, above and behind, or dead astern;
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

"Observed" means seen in game during manual testing. It doesn't mean an automated test covers it.
