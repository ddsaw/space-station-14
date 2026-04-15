using Content.Shared._RedTruce.Lock;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server._RedTruce.Lock;

/// <summary>
/// Handles using keys and keyrings on RTLock entities to lock/unlock them.
/// Subscribes on the TARGET's RTLockComponent via InteractUsingEvent,
/// ordered before SharedStorageSystem to prevent keys being inserted into storage.
/// </summary>
public sealed class RTKeySystem : EntitySystem
{
    [Dependency] private readonly LockSystem _lock = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    private SoundSpecifier _keySound = new SoundPathSpecifier("/Audio/_CP14/Items/lockpick_use.ogg");
    private SoundSpecifier _denySound = new SoundPathSpecifier("/Audio/_CP14/Items/lockpick_fail.ogg");

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RTLockComponent, InteractUsingEvent>(OnInteractUsing,
            before: [typeof(SharedStorageSystem)]);
    }

    private void OnInteractUsing(EntityUid target, RTLockComponent rtLock, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        var used = args.Used;

        if (TryComp<RTKeyComponent>(used, out var keyComp))
        {
            args.Handled = TryUseKeyOnLock(args.User, target, rtLock, keyComp.LockTypes);
            return;
        }

        if (TryComp<RTKeyRingComponent>(used, out _))
        {
            args.Handled = TryUseKeyRingOnLock(args.User, used, target, rtLock);
            return;
        }
    }

    private bool TryUseKeyRingOnLock(EntityUid user, EntityUid keyRingUid, EntityUid target, RTLockComponent rtLock)
    {
        if (!TryComp<LockComponent>(target, out _))
            return false;

        if (!TryComp<StorageComponent>(keyRingUid, out var storage))
            return false;

        foreach (var contained in storage.Container.ContainedEntities)
        {
            if (!TryComp<RTKeyComponent>(contained, out var keyComp))
                continue;

            if (!keyComp.LockTypes.Contains(rtLock.LockType))
                continue;

            return TryUseKeyOnLock(user, target, rtLock, keyComp.LockTypes);
        }

        _popup.PopupEntity(Loc.GetString("rt-lock-keyring-no-match"), target, user);
        _audio.PlayPvs(_denySound, target);
        return true;
    }

    private bool TryUseKeyOnLock(EntityUid user, EntityUid target, RTLockComponent rtLock,
        HashSet<Robust.Shared.Prototypes.ProtoId<RTLockTypePrototype>> keyLockTypes)
    {
        if (!TryComp<LockComponent>(target, out var lockComp))
            return false;

        if (!keyLockTypes.Contains(rtLock.LockType))
        {
            _popup.PopupEntity(Loc.GetString("rt-lock-key-wrong"), target, user);
            _audio.PlayPvs(_denySound, target);
            return true;
        }

        if (lockComp.Locked)
        {
            _lock.Unlock(target, user, lockComp);
            _popup.PopupEntity(Loc.GetString("rt-lock-key-unlock"), target, user);
        }
        else
        {
            _lock.Lock(target, user, lockComp);
            _popup.PopupEntity(Loc.GetString("rt-lock-key-lock"), target, user);
        }

        _audio.PlayPvs(_keySound, target);
        return true;
    }
}
