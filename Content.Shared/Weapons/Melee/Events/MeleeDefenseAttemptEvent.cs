namespace Content.Shared.Weapons.Melee.Events;

/// <summary>
/// High-level defense mode decided by defender-side melee resolvers.
/// </summary>
public enum MeleeDefenseMode : byte
{
    None = 0,
    Parry = 1,
    // Reserved for future defender-side resolution extensions.
    Dodge = 2,
    Shield = 3,
}

/// <summary>
/// Lightweight attack context for defender-side melee defense resolution.
/// </summary>
public enum MeleeAttackType : byte
{
    Light = 0,
    Heavy = 1,
}

/// <summary>
/// Raised on the defender before melee damage is applied.
/// If no system handles this event, no defense is applied.
/// </summary>
public sealed class MeleeDefenseAttemptEvent : EntityEventArgs
{
    public EntityUid Attacker { get; }
    public EntityUid Defender { get; }
    public EntityUid Weapon { get; }
    public MeleeAttackType AttackType { get; }

    /// <summary>
    /// Defense result to drive outcome behavior (e.g. audio/FX).
    /// </summary>
    public MeleeDefenseMode DefenseMode = MeleeDefenseMode.None;

    /// <summary>
    /// If true, melee damage should not be applied to this defender.
    /// </summary>
    public bool SuppressDamage;

    /// <summary>
    /// Set by the defense resolver when it has already emitted outcome audio.
    /// Prevents the shared melee pipeline from playing a second hit sound.
    /// </summary>
    public bool SoundHandled;

    public MeleeDefenseAttemptEvent(EntityUid attacker, EntityUid defender, EntityUid weapon, MeleeAttackType attackType)
    {
        Attacker = attacker;
        Defender = defender;
        Weapon = weapon;
        AttackType = attackType;
    }
}
