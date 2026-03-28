using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce.Rpg;

/// <summary>
/// Per-entity skill category and specialization ratings (baseline + current).
/// </summary>
[RegisterComponent]
public sealed partial class RpgSkillsComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<RpgSkillCategoryPrototype>, RpgStatValue> Categories = new();

    [DataField]
    public Dictionary<ProtoId<RpgSkillSpecializationPrototype>, RpgStatValue> Specializations = new();
}
