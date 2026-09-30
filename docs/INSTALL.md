# Install guide

These steps install NO Missile Indicators by hand on Windows. Paths below are relative to the game folder, the one that holds `NuclearOption.exe`. To find it, right-click Nuclear Option in your Steam library and pick **Manage → Browse local files**. On a default Steam install it's:

```text
C:\Program Files (x86)\Steam\steamapps\common\Nuclear Option
```

## 1. Install BepInEx 5

Skip this step if the game folder already has a `BepInEx` folder with `plugins` inside it.

1. Close Nuclear Option.
2. Download `BepInEx_win_x64_5.4.23.4.zip` from the [BepInEx 5.4.23.4 release](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.4). That's the tested version. Take the `win_x64` file: the game is 64-bit, and BepInEx 6 won't load this mod.
3. Extract the ZIP into the game folder. `winhttp.dll`, `doorstop_config.ini`, and the `BepInEx` folder then sit beside `NuclearOption.exe`.
4. Start the game once and quit at the main menu. That first launch creates `BepInEx/plugins`, `BepInEx/config`, and `BepInEx/LogOutput.log`.

## 2. Install the mod

1. Close Nuclear Option.
2. Download `NoMissileIndicators-v<version>-plugin-only.zip` from the [releases page](https://github.com/baanish/NO-Missile-Indicators/releases).
3. Extract it into the game folder. It holds the folder tree already, so you end up with:

   ```text
   BepInEx/plugins/NoMissileIndicators/NoMissileIndicators.dll
   ```

The release also has `NoMissileIndicators-v<version>-nomm.zip`. That one is flat, for Nuclear Option Mod Manager. To install from it by hand, copy `NoMissileIndicators.dll` to the same path.

## 3. Optional: Configuration Manager for F1

To change settings in game, install [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases). Download the `BepInEx.ConfigurationManager_BepInEx5_v<version>.zip` file, not the IL2CPP one, and extract it into the game folder. Press F1 in game and click **NO Missile Indicators** to open its settings.

Without it, edit the settings file while the game is closed:

```text
BepInEx/config/com.baanish.nuclearoption.missileindicators.cfg
```

BepInEx creates that file the first time the game starts with the mod. The [user guide](USER_GUIDE.md#settings) lists every setting.

## 4. Check that it loaded

Start the game, then open `BepInEx/LogOutput.log` and look for:

```text
[Info   :NO Missile Indicators] NO Missile Indicators 0.1.0 loaded.
```

The version matches the one you installed. If the line is missing, check that the DLL sits at `BepInEx/plugins/NoMissileIndicators/NoMissileIndicators.dll` and that BepInEx wrote its log at all.

In a player-versus-player game the arrows stay off even when the mod has loaded. The log says `Missile arrows hidden: another player is on a hostile faction.` The [user guide](USER_GUIDE.md#off-in-pvp-by-default) explains the rule and the **Allow in PvP** setting.

## Coming from Baanish UI Improvements

Baanish UI Improvements 0.4.0 and earlier drew these arrows themselves. That mod is now [NO Map Overhaul](https://github.com/baanish/NO-Map-Overhaul), which from 0.5.0 on no longer draws arrows.

1. Update Baanish UI Improvements to NO Map Overhaul 0.5.0 or later by following its own install guide. If you don't want the map mod, delete `BepInEx/plugins/BaanishUiImprovements/` instead. Keep one of the two, and you'd otherwise see two arrows per missile.
2. Install NO Missile Indicators as above.
3. Start the game. On its first load, NO Missile Indicators copies **Show arrows** and **Arrow colour** from `BepInEx/config/com.baanish.nuclearoption.uiimprovements.cfg`, and the log says `Copied 2 missile arrow settings`. It only reads the old file. The copy happens once, while the mod has no settings file of its own, so leave the old file in place until then.

## Update

Close the game, download the new `NoMissileIndicators-v<version>-plugin-only.zip`, and extract it into the game folder, replacing the old files. Your settings stay.

## Uninstall

Delete these two:

```text
BepInEx/plugins/NoMissileIndicators/
BepInEx/config/com.baanish.nuclearoption.missileindicators.cfg
```

If Nuclear Option Mod Manager has disabled the mod, its folder is `BepInEx/disabledPlugins/NoMissileIndicators/` instead.
