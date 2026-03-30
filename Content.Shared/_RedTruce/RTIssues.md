# RT Issues

This document tracks known RedTruce issues and development gaps.

If an issue is dealt with (fixed, replaced, or no longer relevant), it must be noted in this document by updating the issue status and adding a short resolution note.

## Status Key

- Open: Not yet resolved.
- In Progress: Actively being worked on.
- Resolved: Completed, with resolution note.

## Current Issues

### 1) Skill/Stat effective rating model

- Status: Open
- Problem: The effective skill/stat rating currently does not reflect a dynamic composite model.
- Expected behavior: Effective rating should be built from a non-determined number of modifiers and update dynamically whenever any contributing stat changes.
- Notes: Needs a flexible modifier pipeline and automatic recomputation/update hooks on stat changes.

### 2) Unimplemented systems (e.g., dicetricks)

- Status: Open
- Problem: Some planned systems are not implemented yet, including dicetricks.
- Expected behavior: Planned RPG subsystems should be implemented and integrated into gameplay flow.
- Notes: Track each subsystem as it enters development to avoid feature drift.

### 3) SS14 native systems not yet disabled

- Status: Open
- Problem: Native SS14 systems that conflict with this project mode are still active.
- Examples: Atmospherics, round-end shuttle, and other default systems.
- Expected behavior: Conflicting native systems should be disabled or replaced where required by project design.
- Notes: Audit startup/system registration and game rules to identify all remaining dependencies.
