# Character UID System Documentation

This document describes how Character UID works as a persistent character identity system.

## Overview

`CharacterUid` is a stable `Guid` attached to each character profile.

It is intended to represent character identity across:

- reconnects
- round restarts
- profile edits
- slot index changes over time

`CharacterUid` is distinct from character slot and character name.

## Identity Model

Character identity and slot identity are intentionally separate:

- slot identity: `profile.slot` (position in a player's character list)
- character identity: `profile.character_uid` (stable unique key)

Slot reuse is allowed. Character UID reuse is not.

## Data Model

### Shared profile model

- Type: `Content.Shared.Preferences.HumanoidCharacterProfile`
- Field: `CharacterUid : Guid`

### Database model

- Type: `Content.Server.Database.Profile`
- Column: `profile.character_uid`
- Constraint: unique index `IX_profile_character_uid`

## Lifecycle Rules

### Character creation

On first insert of a profile row:

- if incoming UID is non-empty, it is persisted
- if incoming UID is empty, server generates a new UID

### Character update

When updating an existing profile row:

- stored `character_uid` is always preserved
- incoming UID is ignored for identity purposes

This prevents client-side UID replacement on existing characters.

### Character deletion and recreation

- deleting a profile row removes its identity
- recreating a character (even in same slot) produces a new UID

### Reconnect and load

- server loads `profile.character_uid`
- it is mapped into `HumanoidCharacterProfile.CharacterUid`
- client receives it through preferences sync
- UI displays the persisted value

## Server Authority Behavior

Authority is row-state based:

- new row: may accept provided non-empty UID
- existing row: must keep stored UID

This combination gives consistent first-save behavior and strong tamper resistance on later updates.

## Migration and Backfill

Both SQLite and Postgres migrations:

1. add `character_uid`
2. backfill existing rows with generated GUID-like values
3. add unique index `IX_profile_character_uid`

SQLite note:

- `NOT NULL` is not enforced through a late-table alter step in this chain
- non-empty values are enforced in runtime save/load paths
- uniqueness is enforced by DB index

## UI Exposure

Character setup UI shows Character UID in the character picker:

- inline short form: `Character ID: <short-id>`
- full value in tooltip

UI behavior is read-only and intended for verification and debugging.

## Behavioral Guarantees

1. Every active character profile has a non-empty UID.
2. UIDs are unique at DB level.
3. Editing a character does not change UID.
4. Reconnecting does not change UID.
5. Delete + recreate produces a different UID.

## Testing Strategy

### Automated

Integration tests verify:

- UID generation and persistence
- tamper resistance on update
- new UID after delete/recreate
- first insert preserving provided non-empty UID

Reference suite: `Content.IntegrationTests.Tests.Preferences.ServerDbSqliteTests`.

### Manual

Use `RTCharacterUid-TestPlaybook.md` for in-game and DB validation steps.

## Usage Guidance for Other Systems

When building systems that need persistent identity:

- use account `UserId` for account-bound state
- use `CharacterUid` for character-bound state

Do not use slot index or character name as persistent character identity keys.

## Troubleshooting

### UID changes after reconnect

Expected behavior: no change.  
If it changes, check creation/update branching in server profile save path and verify existing-row UID preservation.

### Duplicate UID error

Check:

1. migration application state
2. presence of `IX_profile_character_uid`
3. external/manual writes bypassing normal server logic

### UID missing in UI

Check:

1. `profile.character_uid` in DB for that slot
2. preferences sync delivery to client
3. character picker binding of loaded profile data

