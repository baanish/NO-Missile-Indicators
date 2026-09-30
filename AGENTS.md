# Agent notes

Client-side BepInEx 5 mod for Nuclear Option that draws arrows toward incoming missiles. The README covers the feature, `CONTRIBUTING.md` the build and tests, and `docs/DESIGN.md` how an arrow is placed and what the mod may read.

- **Game code:** decompile the installed `Assembly-CSharp.dll` into `.cache/decompiled/` (gitignored) with `ilspycmd`, and read the real type before relying on it.
- **Fair play:** an arrow may only show what the game's own missile warning and HUD marker show. Read `docs/DESIGN.md#what-the-mod-may-show` before changing what the arrows read. Keep the PvP gate (`PvpRule`, `PvpGate`) off by default.
- **Installing:** run `pwsh ./build/Build.ps1 -Install` as a background task. It waits for the game to exit before copying, so start it while Aanish is still playing instead of asking him to report that he closed the game. Keep one queued install at a time: a waiting one copies whatever DLL is newest when the game exits, and several racing for the same file fail.
- **Settings:** BepInEx keeps saved values over new defaults. When you change a default that Aanish should see, delete that key from `BepInEx/config/com.baanish.nuclearoption.missileindicators.cfg` after the game closes. Update the settings table in `docs/USER_GUIDE.md` in the same change.
- **Verification:** unit tests cover the Unity-free logic (arrow placement, tracked positions, the old settings copy, the PvP rule). Everything drawn on screen is checked in game by Aanish from screenshots. Record the result in `docs/TESTING.md`, and update its test count and coverage list when tests change.
- **Releasing:** follow `docs/RELEASING.md`. `pwsh ./build/Release.ps1 -AllowDirty` checks the packages before commit. Publishing (creating the GitHub repository, tag, push, GitHub release, NOMNOM pull request) waits for Aanish's go-ahead.
