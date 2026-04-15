using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce.Lock;

/// <summary>
/// Defines a physical lock type and lockpicking difficulty on an entity that also has LockComponent.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class RTLockComponent : Component
{
    /// <summary>
    /// The type of lock — determines which keys can open it.
    /// </summary>
    [DataField(required: true)]
    [AutoNetworkedField]
    public ProtoId<RTLockTypePrototype> LockType;

    /// <summary>
    /// Number of successes required to pick this lock.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public int Difficulty = 3;

    /// <summary>
    /// Per-die target number for the lockpicking roll (d10).
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public int TargetNumber = 6;
}
