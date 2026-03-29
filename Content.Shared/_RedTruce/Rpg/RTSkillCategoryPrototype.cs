using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce;

[Prototype("rtSkillCategory")]
public sealed partial class RTSkillCategoryPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public int Ordering { get; private set; }

    [DataField(required: true)]
    public string NameLocId { get; private set; } = default!;

    [DataField(required: true)]
    public ProtoId<RTStatPrototype> LinkedStat { get; private set; }
}
