# RT Defender-Side Parry Resolution

This document describes the defender-owned melee resolution flow introduced for RedTruce and explains the audio constraints that came with it.

## Why this exists

Old behavior depended on `MeleeHitEvent` raised on the weapon and ad-hoc suppression in weapon-side handlers.  
New behavior resolves defense on the defender itself so per-target outcomes are explicit and extensible.

## Current flow

1. `SharedMeleeWeaponSystem` identifies candidate targets (light/heavy).
2. For each target, it raises `MeleeDefenseAttemptEvent` on the defender.
3. `RTMeleeOpposedRollSystem` (server) rolls attacker vs defender melee dice pools.
4. On defender win:
  - `args.DefenseMode = MeleeDefenseMode.Parry`
  - `args.SuppressDamage = true`
5. If damage is not suppressed, normal damage + attacked interactions proceed.

If no resolver handles `MeleeDefenseAttemptEvent`, defense defaults to `None` and melee proceeds normally.

## Event contract

`MeleeDefenseAttemptEvent` carries:

- `Attacker`
- `Defender`
- `Weapon`
- `AttackType` (`Light`, `Heavy`)
- `DefenseMode` (`None`, `Parry`, reserved: `Dodge`, `Shield`)
- `SuppressDamage`
- `SoundHandled`

`SoundHandled` exists to prevent duplicate outcome sounds when a resolver plays authoritative audio itself.

## Audio hassle (important)

For server-authoritative defense outcomes (parry vs hit), using shared/predicted hit/parry calls caused double sounds for locally controlled attackers:

- one early/incorrect outcome-like sound
- then the real server outcome sound

This happened because predicted audio and replicated authoritative audio could both occur during prediction/reconciliation windows.

### Required rule for reliable outcome audio

Outcome audio for server-only resolution must be emitted from server-only logic (the resolver) with server-authoritative playback (`PlayPvs`), then mark `args.SoundHandled = true`.

Shared melee fallback hit audio may still exist, but must check `!defenseEvent.SoundHandled` first.

## Extension points

The current model is intentionally minimal and ready for expansion:

- **Dodging**: set `DefenseMode = Dodge`; optionally suppress all damage or apply directional/partial rules.
- **Shield blocks**: set `DefenseMode = Shield`; support stamina drain, shield durability, angle checks.
- **Per-weapon parry sounds**: keep parry sound on the weapon (already possible via `MeleeWeaponComponent.NoDamageSound`) or add dedicated `ParrySound` for distinct tuning.
- **Per-defense VFX**: trigger parry sparks, dodge trails, shield impact visuals based on `DefenseMode`.
- **Resolver layering**: multiple systems may observe `MeleeDefenseAttemptEvent`; define ordering if composing traits, status effects, and equipment rules.

## Addendum: mitigating perceived hit-lag

Because final hit/parry outcome is server-auth and ping-sensitive, there can be a slight delay before outcome audio arrives.

A simple mitigation is to always play predicted swing audio immediately on attack start, including attacks that end up hitting.  
This has been enabled by moving swing playback to the attack-confirmed swing loop (instead of miss-only paths).

Resulting audio model:

- **Immediate local feedback**: swing whoosh (predicted)
- **Authoritative delayed feedback**: parry or hit result (server-resolved)

This separation keeps outcomes correct while preserving responsiveness under latency.

## Addendum: rebuild the ACZ client when testing

Most of these changes live in shared content (`Content.Shared` and related). If you use Hybrid ACZ (launcher-served custom client zip), **you must rebuild and repackage the client** or your local tests will still run stale binaries; several confusing “it does not work” sessions were caused by an outdated `Content.Client.zip`, not by the code itself.

It is generally a good idea to rebuild the ACZ client after any substantive change you are validating in-game. See `Content.Shared/_RedTruce/ACZ-Cursor-Agent-Runbook.md` for the exact packaging flow and copy target (`bin/Content.Server/Content.Client.zip`).