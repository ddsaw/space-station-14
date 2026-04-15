using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Shared._RedTruce.Lock;

/// <summary>
/// Bridges LockComponent and DoorComponent: prevents doors from opening while locked.
/// Also provides user feedback when trying to interact with locked RT storage.
/// </summary>
public sealed class RTDoorLockSystem : EntitySystem
{
    [Dependency] private readonly LockSystem _lock = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    private SoundSpecifier _denySound = new SoundPathSpecifier("/Audio/_CP14/Items/lockpick_use.ogg");

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RTLockComponent, BeforeDoorOpenedEvent>(OnBeforeDoorOpened);
        SubscribeLocalEvent<RTLockComponent, ActivateInWorldEvent>(OnActivateInWorld,
            before: [typeof(LockSystem)]);
    }

    private void OnBeforeDoorOpened(EntityUid uid, RTLockComponent component, BeforeDoorOpenedEvent args)
    {
        if (!_lock.IsLocked(uid))
            return;

        args.Cancel();

        if (args.User is { } user)
        {
            _popup.PopupClient(Loc.GetString("rt-lock-door-locked"), uid, user);
            _audio.PlayPredicted(_denySound, uid, user);
        }
    }

    private void OnActivateInWorld(EntityUid uid, RTLockComponent component, ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        if (!_lock.IsLocked(uid))
            return;

        if (!HasComp<DoorComponent>(uid) && HasComp<StorageComponent>(uid))
        {
            _popup.PopupClient(Loc.GetString("rt-lock-door-locked"), uid, args.User);
            _audio.PlayPredicted(_denySound, uid, args.User);
        }
    }
}
