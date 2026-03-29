using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RedTruce;

[RegisterComponent]
public sealed partial class RTSkillsComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<RTSkillCategoryPrototype>, RTStatValue> Categories = new();

    [DataField]
    public Dictionary<ProtoId<RTSkillSpecializationPrototype>, RTStatValue> Specializations = new();
}
