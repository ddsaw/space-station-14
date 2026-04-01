using Content.Server.Popups;
using Content.Shared._RedTruce;
using Content.Shared.Damage.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._RedTruce.Combat;

/// <summary>
/// Resolves defender-side RT melee opposed rolls and applies parry outcomes.
/// </summary>
public sealed class RTMeleeOpposedRollSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    private ISawmill _sawmill = default!;

    private static readonly ProtoId<RTSkillCategoryPrototype> MeleeCategoryId = new("RTMelee");
    private const int DiceSides = 10;
    private const int TargetNumber = 6;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("redtruce.melee");
        SubscribeLocalEvent<DamageableComponent, MeleeDefenseAttemptEvent>(OnMeleeDefenseAttempt);
    }

    private void OnMeleeDefenseAttempt(Entity<DamageableComponent> defender, ref MeleeDefenseAttemptEvent args)
    {
        if (!_proto.TryIndex(MeleeCategoryId, out RTSkillCategoryPrototype? _))
            return;

        var stats = EntityManager.System<SharedRTStatsSystem>();
        if (Deleted(args.Attacker) || Deleted(defender))
            return;

        var attackerParts = stats.GetSkillDiceContribution(args.Attacker, MeleeCategoryId, null);
        var defenderParts = stats.GetSkillDiceContribution(defender, MeleeCategoryId, null);

        var attackerPool = attackerParts.StatPart + attackerParts.CategoryPart + attackerParts.SpecPart;
        var defenderPool = defenderParts.StatPart + defenderParts.CategoryPart + defenderParts.SpecPart;

        var attackerRoll = RTDicePoolResolver.Roll(_random, new RTDiceRollSpec(attackerPool, DiceSides, TargetNumber));
        var defenderRoll = RTDicePoolResolver.Roll(_random, new RTDiceRollSpec(defenderPool, DiceSides, TargetNumber));

        var parried = defenderRoll.Successes > attackerRoll.Successes;
        if (parried)
        {
            args.DefenseMode = MeleeDefenseMode.Parry;
            args.SuppressDamage = true;
        }

        if (TryComp<MeleeWeaponComponent>(args.Weapon, out var weaponComp))
        {
            var sound = parried ? weaponComp.NoDamageSound : (weaponComp.HitSound ?? weaponComp.NoDamageSound);
            _audio.PlayPvs(sound, defender, sound.Params.WithVariation(0.05f));
            args.SoundHandled = true;
        }

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

        _popup.PopupEntity(msg, args.Attacker, args.Attacker, PopupType.SmallCaution);
        if (defender.Owner != args.Attacker)
            _popup.PopupEntity(msg, defender, defender, PopupType.SmallCaution);

        _sawmill.Info($"{ToPrettyString(args.Attacker)} vs {ToPrettyString(defender)} | {msg}");
    }
}
