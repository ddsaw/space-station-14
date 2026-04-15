using Robust.Shared.GameStates;

namespace Content.Shared._RedTruce.Lock;

/// <summary>
/// Marker component for a keyring. When used on a lock, the system scans
/// the keyring's storage for any RTKeyComponent whose LockTypes match.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class RTKeyRingComponent : Component
{
}
