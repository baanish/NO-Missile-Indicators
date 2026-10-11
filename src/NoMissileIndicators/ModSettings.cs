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

        ShowMissileArrows = Bind("Missile Arrows", "ShowArrows", true, "Show missile arrows", "Point an arrow at each incoming missile outside the view. Only missiles the game's missile warning already knows about get one.");
        Style = Bind("Missile Arrows", "Style", IndicatorStyle.Arrows, "Style", "Arrows: a flashing arrow toward each missile outside the view. ProjectWingman: Project Wingman's 3D indicator, a triangle on a sphere around you toward every warned missile, turned to face it, red on the side facing the missile and yellow behind. The red side shows only for a missile from about half past four round to half past seven; a missile behind you sits below the middle of the screen.");
        RingSize = Bind("Missile Arrows", "RingSize", 30, "Ring size", "Radius of the circle around the screen centre the arrows sit on, or of the ProjectWingman sphere, in percent of the screen height. Arrows never leave the screen, so 200 puts them on the screen edge, even on a 32:9 screen. The sphere stops short of the screen's top and bottom.", acceptable: new AcceptableValueRange<int>(10, 200));
        AllowInPvp = Bind("Missile Arrows", "AllowInPvp", false, "Allow in PvP", "Also show the arrows in player-versus-player games, where another human player isn't on your faction, including one who hasn't picked a side yet. There the arrows may be a competitive advantage, so only turn this on when the other players know and agree.", DrawAllowInPvp);
        MissileArrowColor = Bind("Missile Arrows", "Color", new Color(1f, 0.25f, 0.2f, 1f), "Arrow colour", "Colour the missile arrows flash from, and of the side of a ProjectWingman triangle facing the missile. Red by default.");
        FlashColor = Bind("Missile Arrows", "FlashColor", Color.yellow, "Flash colour", "Colour the missile arrows flash to, as the game's missile warning flashes between yellow and red, and of the back and corner tabs of a ProjectWingman triangle. Yellow by default.");
    }

    public ConfigEntry<bool> Enabled { get; }
    public ConfigEntry<bool> ShowMissileArrows { get; }
    public ConfigEntry<IndicatorStyle> Style { get; }
    public ConfigEntry<int> RingSize { get; }

    /// <summary>Off by default: <see cref="PvpGate"/> hides the arrows in player-versus-player games unless this is on.</summary>
    public ConfigEntry<bool> AllowInPvp { get; }

    public ConfigEntry<Color> MissileArrowColor { get; }
    public ConfigEntry<Color> FlashColor { get; }

    /// <summary>What the arrows are doing right now and why, shown beside Allow in PvP; set by the plugin to <see cref="PvpGate.Status"/>.</summary>
    public System.Func<string>? PvpStatus { get; set; }

    /// <summary>Binds one setting with its F1 name and position.</summary>
    private ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string name, string description,
        System.Action<ConfigEntryBase>? drawer = null, AcceptableValueBase? acceptable = null)
    {
        var display = new ConfigurationManagerAttributes { DispName = name, Order = _order--, CustomDrawer = drawer };
        return _config.Bind(section, key, defaultValue, new ConfigDescription(description, acceptable, display));
    }

    /// <summary>
    /// F1's usual toggle, followed by whether the arrows show in this game, so the PvP rule hiding them never looks like a
    /// broken setting.
    /// </summary>
    private void DrawAllowInPvp(ConfigEntryBase entry)
    {
        AllowInPvp.Value = GUILayout.Toggle(AllowInPvp.Value, AllowInPvp.Value ? "Enabled" : "Disabled", GUILayout.ExpandWidth(false));
        GUILayout.Label(PvpStatus?.Invoke() ?? string.Empty, GUILayout.ExpandWidth(true));
    }
}

/// <summary>How each missile is shown: the game's own arrow, or Project Wingman's 3D triangle.</summary>
internal enum IndicatorStyle
{
    Arrows,
    ProjectWingman,
}
