using Robust.Shared.Serialization;

namespace Content.Shared._RedTruce.CharacterInfo;

/// <summary>
/// Snapshot of character stats sent to the client for the character menu.
/// </summary>
[Serializable, NetSerializable]
public readonly record struct CharacterStatsData(
    int Strength,
    int Agility,
    int Body,
    int Mind,
    int Willpower);
