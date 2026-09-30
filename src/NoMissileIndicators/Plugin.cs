using System.IO;
using BepInEx;
using BepInEx.Configuration;
using NoMissileIndicators.Missiles;

namespace NoMissileIndicators;

/// <summary>
/// Arrows on the screen edge toward incoming missiles outside the view. Nothing in the game is patched: the plugin reads
/// the game's own CombatHUD and missile warning every frame and draws on top, so a game update that moves something
/// fails loudly in the log instead of corrupting game state.
/// </summary>
[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.baanish.nuclearoption.missileindicators";
    public const string PluginName = "NO Missile Indicators";
    public const string PluginVersion = "0.1.0";

    private ModSettings _settings = null!;
    private IncomingMissileArrows _missileArrows = null!;

    private void Awake()
    {
        // BepInEx writes the settings file as the first setting binds, so whether it held anything is read before that.
        var savedBefore = ReadLines(Config.ConfigFilePath);
        _settings = new ModSettings(Config);
        CopyOldSettings(savedBefore);
        _missileArrows = new IncomingMissileArrows(_settings);
        if (!IncomingMissileArrows.ArrowFieldFound)
        {
            Logger.LogWarning("CombatHUD.targetArrow is missing in this game version: no missile arrows.");
        }

        Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }

    /// <summary>
    /// On first load, takes the arrow settings a player chose in Baanish UI Improvements, which shipped the arrows up to
    /// 0.4.0. Reads that mod's settings file and never writes it.
    /// </summary>
    private void CopyOldSettings(string[]? savedBefore)
    {
        var oldConfig = ReadLines(Path.Combine(Paths.ConfigPath, OldSettingsImport.OldConfigFileName));
        var values = OldSettingsImport.ValuesToCopy(savedBefore, oldConfig);
        var copied = 0;
        foreach (var entry in new ConfigEntryBase[] { _settings.ShowMissileArrows, _settings.MissileArrowColor })
        {
            if (values.TryGetValue(entry.Definition.Key, out var value))
            {
                entry.SetSerializedValue(value);
                copied++;
            }
        }

        if (copied > 0)
        {
            Logger.LogInfo($"Copied {copied} missile arrow settings from {OldSettingsImport.OldConfigFileName}.");
        }
    }

    private static string[]? ReadLines(string path) => File.Exists(path) ? File.ReadAllLines(path) : null;

    /// <summary>Per frame, since the arrows follow the camera.</summary>
    private void LateUpdate()
    {
        Guard(() =>
        {
            if (!_settings.Enabled.Value)
            {
                // A no-op once the arrows are gone, so the switch takes effect the frame it flips in F1. The arrows
                // rebuild on their next render, so switching back on needs nothing more.
                _missileArrows.Reset();
                return;
            }

            _missileArrows.Render();
        });
    }

    /// <summary>A throw disables the plugin rather than repeating every frame.</summary>
    private void Guard(System.Action action)
    {
        if (!enabled)
        {
            return;
        }

        try
        {
            action();
        }
        catch (System.Exception exception)
        {
            Logger.LogError($"Disabled after an error; the game is unaffected. {exception}");
            enabled = false;
            try
            {
                _missileArrows.Reset();
            }
            catch (System.Exception cleanup)
            {
                Logger.LogError($"Couldn't undo everything after that error. {cleanup}");
            }
        }
    }

    private void OnDestroy() => _missileArrows.Reset();
}
