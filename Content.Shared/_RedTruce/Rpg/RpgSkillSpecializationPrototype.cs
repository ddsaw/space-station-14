using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce.Rpg;

[Prototype("rpgSkillSpecialization")]
public sealed partial class RpgSkillSpecializationPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public int Ordering { get; private set; }

    [DataField(required: true)]
    public string NameLocId { get; private set; } = default!;

    [DataField(required: true)]
    public ProtoId<RpgSkillCategoryPrototype> Category { get; private set; }
}
