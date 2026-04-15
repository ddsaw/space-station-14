using Content.Shared._RedTruce;
using Content.Shared._RedTruce.Lock;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._RedTruce.Lock;

/// <summary>
/// Handles lockpick use on RTLock entities: do-after then skill check to unlock.
/// Subscribes via AfterInteractEvent on the USED lockpick component.
/// This works because locked storage fails CanInteract (via LockedStorageComponent),
/// so the storage system doesn't consume the event.
/// </summary>
public sealed class RTLockpickSystem : EntitySystem
{
    [Dependency] private readonly LockSystem _lock = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedRTStatsSystem _stats = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    private static readonly ProtoId<RTSkillCategoryPrototype> SecurityCategory = "RTSecurity";
    private static readonly ProtoId<RTSkillSpecializationPrototype> LockpickingSpec = "RTLockpicking";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RTLockpickComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<RTLockComponent, RTLockpickDoAfterEvent>(OnLockpickDoAfter);
    }

    private void OnAfterInteract(EntityUid uid, RTLockpickComponent lockpick, AfterInteractEvent args)
    {
        if (args.Handled || args.Target is not { } target || !args.CanReach)
            return;

        if (!TryComp<RTLockComponent>(target, out _))
            return;

        if (!TryComp<LockComponent>(target, out var lockComp) || !lockComp.Locked)
            return;

        args.Handled = true;

        _popup.PopupEntity(Loc.GetString("rt-lockpick-start"), target, args.User);
        _audio.PlayPvs(lockpick.SuccessSound, target);

        var doAfter = new DoAfterArgs(EntityManager, args.User, lockpick.PickTime,
            new RTLockpickDoAfterEvent(GetNetEntity(uid)), target, uid)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            NeedHand = true,
            BreakOnDropItem = true,
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnLockpickDoAfter(EntityUid target, RTLockComponent rtLock, RTLockpickDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        var lockpickUid = GetEntity(args.LockpickNetEntity);
        if (!TryComp<RTLockpickComponent>(lockpickUid, out var lockpick))
            return;

        if (!TryComp<LockComponent>(target, out var lockComp) || !lockComp.Locked)
            return;

        var breakdown = _stats.GetSkillDiceContribution(args.User, SecurityCategory, LockpickingSpec);
        var pool = breakdown.StatPart + breakdown.CategoryPart + breakdown.SpecPart;
        var spec = new RTDiceRollSpec(pool, 10, rtLock.TargetNumber);
        var result = RTDicePoolResolver.Roll(_random, spec);

        if (result.Successes >= rtLock.Difficulty)
        {
            _lock.Unlock(target, args.User, lockComp);
            _popup.PopupEntity(Loc.GetString("rt-lockpick-success",
                ("successes", result.Successes), ("needed", rtLock.Difficulty)), target, args.User);
            _audio.PlayPvs(lockpick.SuccessSound, target);
        }
        else
        {
            _popup.PopupEntity(Loc.GetString("rt-lockpick-fail",
                ("successes", result.Successes), ("needed", rtLock.Difficulty)), target, args.User);
            _audio.PlayPvs(lockpick.FailSound, target);

            lockpick.CurrentDurability--;
            Dirty(lockpickUid, lockpick);

            if (lockpick.CurrentDurability <= 0)
            {
                _popup.PopupEntity(Loc.GetString("rt-lockpick-break"), target, args.User);
                _audio.PlayPvs(lockpick.BreakSound, target);
                QueueDel(lockpickUid);
            }
        }
    }
}
