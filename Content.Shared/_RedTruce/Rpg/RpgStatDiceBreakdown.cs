using Robust.Shared.Serialization;

namespace Content.Shared._RedTruce.Rpg;

/// <summary>
/// Parts of a skill dice pool from stat, category, and optional specialization.
/// </summary>
[Serializable, NetSerializable]
public readonly record struct RpgStatDiceBreakdown(int StatPart, int CategoryPart, int SpecPart);
