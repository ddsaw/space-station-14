using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce.Rpg;

/// <summary>
/// Global RedTruce defaults when an entity has no component data and no species match.
/// </summary>
[Prototype("rpgGlobalFallback")]
public sealed partial class RpgGlobalFallbackPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public Dictionary<ProtoId<RpgStatPrototype>, RpgStatValue> Stats = new();

    [DataField]
    public Dictionary<ProtoId<RpgSkillCategoryPrototype>, RpgStatValue> SkillCategories = new();

    [DataField]
    public Dictionary<ProtoId<RpgSkillSpecializationPrototype>, RpgStatValue> SkillSpecializations = new();
}
