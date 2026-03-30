# Character UID Test Playbook

This verifies `CharacterUid` generation, persistence, and UI visibility.
For architecture and behavior details, see `RTCharacterUid-Documentation.md`.

## Preconditions

- Test on both SQLite and Postgres configurations.
- Start with an account that can create/delete character slots.

## In-game checks

1. Open character setup and create two character slots.
2. Confirm both slots show a `Character ID` line and the IDs are different.
3. Edit first character (name, age, appearance), save, then re-open setup.
4. Confirm first character's `Character ID` did not change.
5. Delete one character slot and create a new one in that slot.
6. Confirm the recreated slot has a new `Character ID`.
7. Restart the round/reconnect and verify all remaining IDs are stable.

## Data integrity checks

1. Ensure no slot renders an empty/missing ID.
2. Verify no duplicate IDs appear for characters on the same account.
3. (Optional DB query) Validate `profile.character_uid` is non-null and unique.

## Expected result

- Every character has a non-empty unique `CharacterUid`.
- Character edits preserve `CharacterUid`.
- Slot recreation generates a new `CharacterUid`.
- UI display matches persisted identity across reconnects.

