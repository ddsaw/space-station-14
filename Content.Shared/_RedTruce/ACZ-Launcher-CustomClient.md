# ACZ, launcher, and content-only shader notes

This note documents the final working approach for launcher-compatible shadow grain, what failed, and how to harden settings for live servers.

---

## Current working architecture

- Use a **content-side fullscreen shader overlay** (`Resources/Textures/Shaders/light_shadow_grain_fullscreen.swsl` + `Content.Client/Light/ShadowGrainOverlay.cs`).
- Drive it through **content CVars** in `Content.Shared/CCVar/CCVars.Lighting.cs`.
- Keep all logic in content assemblies/resources so launcher clients receive it through ACZ.

This avoids relying on engine-internal light shaders for gameplay visuals.

---

## Why engine-side shader edits were removed

Engine path experiments (e.g. editing `RobustToolbox/Resources/Shaders/Internal/light_shared.swsl`) were not reliable for launcher clients:

- launcher runs installed engine binaries/resources first
- same-path engine shader files may resolve before downloaded ACZ content
- result: CVar appeared, but expected shader behavior did not consistently apply

For this fork, engine-side debug/noise hooks were removed to keep only the working content workaround.

---

## Default grain settings (current fork defaults)

These are now the defaults in `CCVars.Lighting.cs`:

```text
light.shadow_grain_strength               0.018
light.shadow_grain_scale                  1500000
light.shadow_grain_dark_threshold         0.45
light.shadow_grain_dark_threshold_max     0.80
light.shadow_grain_disable_threshold      0.0
light.shadow_grain_animation_speed        0.0
light.shadow_grain_flicker_strength       0.08
```

`light.shadow_grain_debug` remains available for visual probing.

---

## Known non-functioning/ambiguous behavior

- `light.shadow_grain_dark_threshold` and `light.shadow_grain_dark_threshold_max` may appear to do nothing with some combinations, especially with the current defaults.
- In particular, interaction with `light.shadow_grain_disable_threshold` can dominate perceived output and make dark-threshold tuning visually subtle or masked.

This is currently accepted as-is for the present aesthetic goal.

---

## ACZ / launcher process (still required)

Because this is content-driven, launcher users must still receive the updated client package:

1. Build package:

```text
dotnet run --project Content.Packaging --configuration Release -- client
```

2. Place zip for Hybrid ACZ:

```text
release/SS14.Client.zip -> bin/Content.Server/Content.Client.zip
```

3. Launch server with local ACZ and bump version:

```text
dotnet run --project Content.Server -- --cvar build.download_url= --cvar build.manifest_url= --cvar build.fork_id=redtruce --cvar build.version=<new-id>
```

Always change `build.version` when shipping new client content so launcher cache refreshes.

---

## Plan: disable client-side tuning on live servers

Current state keeps cvar tuning enabled for iteration.  
When ready for production, use one of these:

1. **Hardcode values in overlay code** (no runtime knobs).
2. **Keep CVars but disallow live edits** (e.g. not-connected usage only).
3. **Server-authoritative config flow** (recommended): replicated/server-controlled values, client cannot tune while connected.

Given the threat model (small community, no custom client expected), option 3 is usually the best tradeoff.

---

## Pitfalls reminder

- If `Platform`/`PLATFORM` env var is set (e.g. `HPD`), build output paths can break packaging assumptions.
- If port 1212 is in use, server start fails with bind error.
- Keep server stopped while rebuilding if file locks occur.

---

*Updated for final content-only grain workflow.*
