using Content.Server.Popups;
using Content.Shared._RedTruce;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._RedTruce.Combat;

/// <summary>
/// Phase A MVP hook: rolls opposed RT melee pools on hit and can suppress damage per target.
/// </summary>
public sealed class RTMeleeOpposedRollSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;

    private static readonly ProtoId<RTSkillCategoryPrototype> MeleeCategoryId = new("RTMelee");
    private const int DiceSides = 10;
    private const int TargetNumber = 6;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("redtruce.melee");
        SubscribeLocalEvent<MeleeWeaponComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnMeleeHit(EntityUid uid, MeleeWeaponComponent component, MeleeHitEvent args)
    {
        if (!args.IsHit || args.HitEntities.Count == 0)
            return;

        if (!_proto.TryIndex(MeleeCategoryId, out RTSkillCategoryPrototype? _))
            return;

        var stats = EntityManager.System<SharedRTStatsSystem>();

        foreach (var target in args.HitEntities)
        {
            if (Deleted(target))
                continue;

            var attackerParts = stats.GetSkillDiceContribution(args.User, MeleeCategoryId, null);
            var defenderParts = stats.GetSkillDiceContribution(target, MeleeCategoryId, null);

            var attackerPool = attackerParts.StatPart + attackerParts.CategoryPart + attackerParts.SpecPart;
            var defenderPool = defenderParts.StatPart + defenderParts.CategoryPart + defenderParts.SpecPart;

            var attackerRoll = RTDicePoolResolver.Roll(_random, new RTDiceRollSpec(attackerPool, DiceSides, TargetNumber));
            var defenderRoll = RTDicePoolResolver.Roll(_random, new RTDiceRollSpec(defenderPool, DiceSides, TargetNumber));

            var parried = defenderRoll.Successes > attackerRoll.Successes;
            if (parried)
                args.SuppressDamageTargets.Add(target);

            var attackerFaces = attackerRoll.Faces.Count == 0 ? "-" : string.Join(", ", attackerRoll.Faces);
            var defenderFaces = defenderRoll.Faces.Count == 0 ? "-" : string.Join(", ", defenderRoll.Faces);
            var outcome = Loc.GetString(parried ? "rt-melee-roll-outcome-parried" : "rt-melee-roll-outcome-hit");
            var msg = Loc.GetString("rt-melee-roll-debug",
                ("attackerPool", attackerPool),
                ("defenderPool", defenderPool),
                ("tn", TargetNumber),
                ("attackerSuccesses", attackerRoll.Successes),
                ("defenderSuccesses", defenderRoll.Successes),
                ("attackerFaces", attackerFaces),
                ("defenderFaces", defenderFaces),
                ("outcome", outcome));

            _popup.PopupEntity(msg, args.User, args.User, PopupType.SmallCaution);
            if (target != args.User)
                _popup.PopupEntity(msg, target, target, PopupType.SmallCaution);

            _sawmill.Info($"{ToPrettyString(args.User)} vs {ToPrettyString(target)} | {msg}");
        }
    }
}
