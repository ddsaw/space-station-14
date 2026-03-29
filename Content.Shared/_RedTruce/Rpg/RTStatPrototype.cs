using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce;

[Prototype("rtStat")]
public sealed partial class RTStatPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Sort order in character sheet (lower first).
    /// </summary>
    [DataField]
    public int Ordering { get; private set; }

    /// <summary>
    /// Fluent key for the stat name (e.g. rpg-stat-strength).
    /// </summary>
    [DataField(required: true)]
    public string NameLocId { get; private set; } = default!;
}
