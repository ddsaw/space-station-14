using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._RedTruce.Lock;

[Serializable, NetSerializable]
public sealed partial class RTLockpickDoAfterEvent : DoAfterEvent
{
    public NetEntity LockpickNetEntity;

    public RTLockpickDoAfterEvent(NetEntity lockpickNetEntity)
    {
        LockpickNetEntity = lockpickNetEntity;
    }

    private RTLockpickDoAfterEvent() { }

    public override DoAfterEvent Clone()
    {
        return new RTLockpickDoAfterEvent(LockpickNetEntity);
    }
}
