using System.Collections.Generic;
using BepInEx.Logging;
using NuclearOption.Networking;
using UnityEngine;

namespace NoMissileIndicators;

/// <summary>
/// Hides the arrows in player-versus-player games unless Allow in PvP is on, and when it can't tell (<see cref="PvpRule"/>).
/// Reads the game's player list once a second rather than every frame: players join, leave, and pick factions far more
/// slowly than that. Logs once when the arrows go and once when they come back.
/// </summary>
internal sealed class PvpGate
{
    private const float CheckSeconds = 1f;

    private readonly ModSettings _settings;
    private readonly ManualLogSource _logger;
    private readonly List<object?> _otherPlayers = new();
    private PvpVerdict _verdict;
    private float _nextCheck;
    private bool _hiding;

    public PvpGate(ModSettings settings, ManualLogSource logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public bool HidesArrows()
    {
        if (Time.unscaledTime >= _nextCheck)
        {
            _nextCheck = Time.unscaledTime + CheckSeconds;
            _verdict = Judge();
        }

        var hiding = _verdict != PvpVerdict.NotPvp && !_settings.AllowInPvp.Value;
        if (hiding != _hiding)
        {
            _hiding = hiding;
            _logger.LogInfo(hiding switch
            {
                true when _verdict == PvpVerdict.Pvp => "Missile arrows hidden: another player is on a hostile faction. Missile Arrows > Allow in PvP overrides this.",
                true => "Missile arrows hidden until the mod can tell whether this is a player-versus-player game.",
                false when _settings.AllowInPvp.Value => "Missile arrows shown: Allow in PvP is on.",
                false => "Missile arrows shown: no other player is on a hostile faction.",
            });
        }

        return hiding;
    }

    /// <summary>
    /// Every player the game knows on this client is in <see cref="UnitRegistry.playerLookup"/>, added as each one's
    /// network object starts; <see cref="Player.HQ"/> is null until the player picks a faction.
    /// </summary>
    private PvpVerdict Judge()
    {
        var multiplayer = GameManager.gameState == GameState.Multiplayer;
        if (!multiplayer)
        {
            return PvpRule.Judge(multiplayer, null, null);
        }

        GameManager.GetLocalPlayer<Player>(out var local);
        _otherPlayers.Clear();
        foreach (var player in UnitRegistry.playerLookup.Values)
        {
            if (player != null && player != local)
            {
                _otherPlayers.Add(player.HQ != null ? player.HQ : null);
            }
        }

        var localFaction = local != null && local.HQ != null ? local.HQ : null;
        return PvpRule.Judge(multiplayer, localFaction, local != null ? _otherPlayers : null);
    }
}
