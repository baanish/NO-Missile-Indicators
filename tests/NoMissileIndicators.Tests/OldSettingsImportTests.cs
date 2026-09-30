using static NoMissileIndicators.Tests.Program;

namespace NoMissileIndicators.Tests;

/// <summary>Taking the missile arrow settings from Baanish UI Improvements' file on first load.</summary>
internal static class OldSettingsImportTests
{
    public static readonly (string Name, Action Test)[] All =
    {
        ("a real 0.4.0 file gives its two arrow settings and nothing else", RealOldFileGivesArrowSettings),
        ("the grouped 0.5.0 section copies too", GroupedSectionCopies),
        ("the grouped section wins over the 0.4.0 one", GroupedSectionWins),
        ("a saved value of this mod's own stops the copy", OwnSavedValueStopsCopy),
        ("a settings file with no values yet still copies", EmptyOwnFileStillCopies),
        ("no old file copies nothing", NoOldFileCopiesNothing),
    };

    private static readonly string[] Grouped =
    {
        "[General]",
        "Enabled = true",
        "",
        "[HUD / Missile Arrows]",
        "## Colour of the missile arrows.",
        "# Setting type: Color",
        "# Default value: FF4033FF",
        "Color = 00FFFFFF",
        "ShowArrows = false",
    };

    private static void RealOldFileGivesArrowSettings()
    {
        var old = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "Fixtures", "settings-0.4.0.cfg"));
        var copy = OldSettingsImport.ValuesToCopy(null, old);
        Expect(copy.Count == 2, $"expected 2 values, got {string.Join(", ", copy.Keys)}");
        ExpectText(copy["ShowArrows"], "true");
        ExpectText(copy["Color"], "FF4033FF");
    }

    private static void GroupedSectionCopies()
    {
        var copy = OldSettingsImport.ValuesToCopy(null, Grouped);
        Expect(copy.Count == 2, $"expected 2 values, got {string.Join(", ", copy.Keys)}");
        ExpectText(copy["ShowArrows"], "false");
        ExpectText(copy["Color"], "00FFFFFF");
    }

    private static void GroupedSectionWins()
    {
        var both = new[] { "[Missile Arrows]", "ShowArrows = true", "Color = FF4033FF" }.Concat(Grouped);
        var copy = OldSettingsImport.ValuesToCopy(null, both);
        ExpectText(copy["ShowArrows"], "false");
        ExpectText(copy["Color"], "00FFFFFF");
    }

    private static void OwnSavedValueStopsCopy()
    {
        var own = new[] { "## Settings file was created by plugin NO Missile Indicators v0.1.0", "[Missile Arrows]", "ShowArrows = true" };
        Expect(OldSettingsImport.ValuesToCopy(own, Grouped).Count == 0, "expected nothing copied over a saved value");
    }

    private static void EmptyOwnFileStillCopies()
    {
        var own = new[] { "## Settings file was created by plugin NO Missile Indicators v0.1.0", "" };
        Expect(OldSettingsImport.ValuesToCopy(own, Grouped).Count == 2, "expected both values copied");
    }

    private static void NoOldFileCopiesNothing() =>
        Expect(OldSettingsImport.ValuesToCopy(null, null).Count == 0, "expected nothing without the old file");
}
