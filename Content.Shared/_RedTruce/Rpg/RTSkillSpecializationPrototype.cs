using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce;

[Prototype("rtSkillSpecialization")]
public sealed partial class RTSkillSpecializationPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public int Ordering { get; private set; }

    [DataField(required: true)]
    public string NameLocId { get; private set; } = default!;

    [DataField(required: true)]
    public ProtoId<RTSkillCategoryPrototype> Category { get; private set; }
}
