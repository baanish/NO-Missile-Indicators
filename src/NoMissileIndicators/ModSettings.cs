using BepInEx.Configuration;
using UnityEngine;

namespace NoMissileIndicators;

/// <summary>
/// Every player-facing knob, editable in BepInEx/config or live through ConfigurationManager (F1). F1 lists sections in
/// the order they are first bound and settings in bind order within a section, so this constructor reads top to bottom
/// like the window.
/// </summary>
internal sealed class ModSettings
{
    private readonly ConfigFile _config;

    /// <summary>F1 lists higher Order first, so counting down keeps bind order.</summary>
    private int _order = 1000;

    public ModSettings(ConfigFile config)
    {
        _config = config;

        Enabled = Bind("General", "Enabled", true, "Enabled", "Off hides everything this mod draws, as if it were not installed. Useful for checking whether a problem comes from the mod or the game.");

        ShowMissileArrows = Bind("Missile Arrows", "ShowArrows", true, "Show missile arrows", "Point an arrow from the screen edge at each incoming missile outside the view. Only missiles the game's missile warning already knows about get one.");
        AllowInPvp = Bind("Missile Arrows", "AllowInPvp", false, "Allow in PvP", "Also show the arrows in player-versus-player games, where another human player is on a hostile faction. There the arrows may be a competitive advantage, so only turn this on when the other players know and agree.");
        MissileArrowColor = Bind("Missile Arrows", "Color", new Color(1f, 0.25f, 0.2f, 1f), "Arrow colour", "Colour of the missile arrows.");
    }

    public ConfigEntry<bool> Enabled { get; }
    public ConfigEntry<bool> ShowMissileArrows { get; }

    /// <summary>Off by default: <see cref="PvpGate"/> hides the arrows in player-versus-player games unless this is on.</summary>
    public ConfigEntry<bool> AllowInPvp { get; }

    public ConfigEntry<Color> MissileArrowColor { get; }

    /// <summary>Binds one setting with its F1 name and position.</summary>
    private ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string name, string description)
    {
        var display = new ConfigurationManagerAttributes { DispName = name, Order = _order-- };
        return _config.Bind(section, key, defaultValue, new ConfigDescription(description, null, display));
    }
}
