using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce.Rpg;

/// <summary>
/// Default RPG stats/skills for a species. Prototype id should match <see cref="Species"/> for lookup.
/// </summary>
[Prototype("rpgSpeciesStatsDefaults")]
public sealed partial class RpgSpeciesStatsDefaultsPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public ProtoId<SpeciesPrototype> Species { get; private set; }

    [DataField]
    public Dictionary<ProtoId<RpgStatPrototype>, RpgStatValue> Stats = new();

    [DataField]
    public Dictionary<ProtoId<RpgSkillCategoryPrototype>, RpgStatValue> SkillCategories = new();

    [DataField]
    public Dictionary<ProtoId<RpgSkillSpecializationPrototype>, RpgStatValue> SkillSpecializations = new();
}
