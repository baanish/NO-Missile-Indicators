using static NoMissileIndicators.Tests.Program;

namespace NoMissileIndicators.Tests;

/// <summary>Whether a game counts as player versus player, from your faction and the other human players' factions.</summary>
internal static class PvpRuleTests
{
    public static readonly (string Name, Action Test)[] All =
    {
        ("single player is not PvP", SinglePlayerIsNotPvp),
        ("co-op against AI is not PvP", CoopAgainstAiIsNotPvp),
        ("other humans only on your side is not PvP", HumansOnlyOnYourSideIsNotPvp),
        ("one hostile human is PvP", OneHostileHumanIsPvp),
        ("a player changing faction changes the verdict", FactionChangeChangesVerdict),
        ("a player with no faction yet is not hostile", NoFactionYetIsNotHostile),
        ("an unreadable faction or player list is unknown", UnreadableIsUnknown),
    };

    private static void SinglePlayerIsNotPvp() =>
        ExpectVerdict(PvpRule.Judge(false, "Boscali", new object?[] { "Primeva" }), PvpVerdict.NotPvp);

    private static void CoopAgainstAiIsNotPvp() =>
        ExpectVerdict(PvpRule.Judge(true, "Boscali", Array.Empty<object?>()), PvpVerdict.NotPvp);

    private static void HumansOnlyOnYourSideIsNotPvp() =>
        ExpectVerdict(PvpRule.Judge(true, "Boscali", new object?[] { "Boscali", "Boscali" }), PvpVerdict.NotPvp);

    private static void OneHostileHumanIsPvp() =>
        ExpectVerdict(PvpRule.Judge(true, "Boscali", new object?[] { "Boscali", "Primeva" }), PvpVerdict.Pvp);

    private static void FactionChangeChangesVerdict()
    {
        ExpectVerdict(PvpRule.Judge(true, "Boscali", new object?[] { "Primeva" }), PvpVerdict.Pvp);
        ExpectVerdict(PvpRule.Judge(true, "Primeva", new object?[] { "Primeva" }), PvpVerdict.NotPvp);
    }

    private static void NoFactionYetIsNotHostile() =>
        ExpectVerdict(PvpRule.Judge(true, "Boscali", new object?[] { null }), PvpVerdict.NotPvp);

    private static void UnreadableIsUnknown()
    {
        ExpectVerdict(PvpRule.Judge(true, null, new object?[] { "Boscali" }), PvpVerdict.Unknown);
        ExpectVerdict(PvpRule.Judge(true, "Boscali", null), PvpVerdict.Unknown);
    }

    private static void ExpectVerdict(PvpVerdict actual, PvpVerdict expected) =>
        Expect(actual == expected, $"expected {expected}, got {actual}");
}
