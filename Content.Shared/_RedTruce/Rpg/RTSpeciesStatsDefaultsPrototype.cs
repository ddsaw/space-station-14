using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce;

[Prototype("rtSpeciesStatsDefaults")]
public sealed partial class RTSpeciesStatsDefaultsPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public ProtoId<SpeciesPrototype> Species { get; private set; }

    [DataField]
    public Dictionary<ProtoId<RTStatPrototype>, RTStatValue> Stats = new();

    [DataField]
    public Dictionary<ProtoId<RTSkillCategoryPrototype>, RTStatValue> SkillCategories = new();

    [DataField]
    public Dictionary<ProtoId<RTSkillSpecializationPrototype>, RTStatValue> SkillSpecializations = new();
}
