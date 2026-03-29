using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce;

[Prototype("rtGlobalFallback")]
public sealed partial class RTGlobalFallbackPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public Dictionary<ProtoId<RTStatPrototype>, RTStatValue> Stats = new();

    [DataField]
    public Dictionary<ProtoId<RTSkillCategoryPrototype>, RTStatValue> SkillCategories = new();

    [DataField]
    public Dictionary<ProtoId<RTSkillSpecializationPrototype>, RTStatValue> SkillSpecializations = new();
}
