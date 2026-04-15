using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce.Lock;

/// <summary>
/// A physical key that can open locks of matching types.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class RTKeyComponent : Component
{
    /// <summary>
    /// Which lock types this key can open.
    /// </summary>
    [DataField(required: true)]
    [AutoNetworkedField]
    public HashSet<ProtoId<RTLockTypePrototype>> LockTypes = new();
}
