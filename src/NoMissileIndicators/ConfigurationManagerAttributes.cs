namespace NoMissileIndicators;

/// <summary>
/// The fields this mod uses from BepInEx ConfigurationManager's standard attributes class. ConfigurationManager finds a
/// setting's tag by this exact type name and copies the public fields it knows, so the mod needs no reference to it.
/// Without ConfigurationManager the tag does nothing.
/// </summary>
internal sealed class ConfigurationManagerAttributes
{
    /// <summary>The name F1 shows instead of the config key.</summary>
    public string? DispName;

    /// <summary>Position within the section: F1 lists higher numbers first.</summary>
    public int? Order;

    /// <summary>Draws the setting's value in place of F1's own control.</summary>
    public System.Action<BepInEx.Configuration.ConfigEntryBase>? CustomDrawer;
}
