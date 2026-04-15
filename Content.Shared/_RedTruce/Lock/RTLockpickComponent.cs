using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._RedTruce.Lock;

/// <summary>
/// A lockpick tool that can be used to attempt picking an RTLock via skill check.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class RTLockpickComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public int MaxDurability = 5;

    [DataField]
    [AutoNetworkedField]
    public int CurrentDurability = 5;

    /// <summary>
    /// How long the lockpicking do-after takes.
    /// </summary>
    [DataField]
    public TimeSpan PickTime = TimeSpan.FromSeconds(5);

    [DataField]
    public SoundSpecifier? SuccessSound;

    [DataField]
    public SoundSpecifier? FailSound;

    [DataField]
    public SoundSpecifier? BreakSound;
}
