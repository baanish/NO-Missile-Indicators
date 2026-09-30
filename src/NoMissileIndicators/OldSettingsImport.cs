using System.Collections.Generic;

namespace NoMissileIndicators;

/// <summary>
/// Carries the missile arrow settings over from Baanish UI Improvements, which shipped the arrows before this mod did.
/// Works on the lines of the two settings files, so it's free of Unity and BepInEx, and never writes the old file.
/// </summary>
internal static class OldSettingsImport
{
    public const string OldConfigFileName = "com.baanish.nuclearoption.uiimprovements.cfg";

    /// <summary>
    /// Where the old mod saved the arrow settings: "Missile Arrows" in 0.4.0, "HUD / Missile Arrows" once its 0.5.0 work
    /// grouped the sections. The later layout comes first and wins. Keys are the same as this mod's.
    /// </summary>
    internal static readonly string[] OldSections = { "HUD / Missile Arrows", "Missile Arrows" };

    /// <summary>
    /// The old arrow values to copy, by key. Empty unless this mod has no saved value yet (<paramref name="newConfig"/>
    /// is null when its file doesn't exist) and the old file (null when missing) saved some.
    /// </summary>
    public static Dictionary<string, string> ValuesToCopy(IEnumerable<string>? newConfig, IEnumerable<string>? oldConfig)
    {
        var copy = new Dictionary<string, string>();
        if (oldConfig is null || (newConfig is not null && ReadCfg(newConfig).Count > 0))
        {
            return copy;
        }

        var old = ReadCfg(oldConfig);
        foreach (var section in OldSections)
        {
            foreach (var entry in old)
            {
                if (entry.Key.Section == section && !copy.ContainsKey(entry.Key.Key))
                {
                    copy[entry.Key.Key] = entry.Value;
                }
            }
        }

        return copy;
    }

    /// <summary>Sections and "key = value" lines, as BepInEx reads them; comments and blank lines skipped.</summary>
    internal static Dictionary<(string Section, string Key), string> ReadCfg(IEnumerable<string> lines)
    {
        var values = new Dictionary<(string, string), string>();
        var section = "";
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("#"))
            {
                continue;
            }

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                section = line.Substring(1, line.Length - 2);
                continue;
            }

            var split = line.IndexOf('=');
            if (split > 0)
            {
                values[(section, line.Substring(0, split).Trim())] = line.Substring(split + 1).Trim();
            }
        }

        return values;
    }
}
