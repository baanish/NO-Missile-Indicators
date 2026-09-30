using System.Collections.Generic;

namespace NoMissileIndicators;

public enum PvpVerdict
{
    /// <summary>Single player, co-op against AI, or other humans only on your side.</summary>
    NotPvp,

    /// <summary>At least one other human isn't on your faction: on another one, or not on one yet.</summary>
    Pvp,

    /// <summary>A multiplayer game where the mod can't read your faction or the player list.</summary>
    Unknown,
}

/// <summary>
/// Whether a game is player versus player: multiplayer, with at least one other human who isn't on your faction.
/// Any other faction counts as hostile, as it does for the game when it adds AI aircraft per enemy player
/// (<c>FactionHQ.DeployAIAircraft</c>). So does a human who hasn't picked a faction yet, still joining or watching: they
/// may well pick the other side, and the arrows mustn't be on when they do. Factions are compared with <see cref="object.Equals(object, object)"/>, so the
/// caller passes the game's faction objects and the tests pass names.
/// </summary>
public static class PvpRule
{
    /// <param name="multiplayer">
    /// The game's own multiplayer state (<c>GameState.Multiplayer</c>), which both host and client are in. Single player
    /// and the mission editor have no other players.
    /// </param>
    /// <param name="localFaction">Your faction; null when you haven't picked one or it can't be read.</param>
    /// <param name="otherPlayers">
    /// The faction of every other human player, null for one who hasn't picked a faction yet. The list itself is null
    /// when it can't be read.
    /// </param>
    public static PvpVerdict Judge(bool multiplayer, object? localFaction, IEnumerable<object?>? otherPlayers)
    {
        if (!multiplayer)
        {
            return PvpVerdict.NotPvp;
        }

        if (localFaction is null || otherPlayers is null)
        {
            return PvpVerdict.Unknown;
        }

        foreach (var faction in otherPlayers)
        {
            if (!Equals(faction, localFaction))
            {
                return PvpVerdict.Pvp;
            }
        }

        return PvpVerdict.NotPvp;
    }
}
