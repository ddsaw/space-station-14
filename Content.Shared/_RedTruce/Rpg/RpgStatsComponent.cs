using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RedTruce.Rpg;

/// <summary>
/// Per-entity RPG stat baseline and current values. Server-authoritative; the client receives snapshots via character info.
/// </summary>
[RegisterComponent]
public sealed partial class RpgStatsComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<RpgStatPrototype>, RpgStatValue> Stats = new();
}
