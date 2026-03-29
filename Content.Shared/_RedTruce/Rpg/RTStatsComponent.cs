using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RedTruce;

/// <summary>
/// Per-entity stat baseline and current values. Server-authoritative; the client receives snapshots via character info.
/// </summary>
[RegisterComponent]
public sealed partial class RTStatsComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<RTStatPrototype>, RTStatValue> Stats = new();
}
