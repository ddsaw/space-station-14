# RedTruce cone vision (`rt.cone_vision`)

Content-only client feature: a forward **vision cone** (black outside the cone on top of the normal render) plus a **near circle** around the player that stays fully visible. Facing uses the **local player’s** `TransformComponent` world rotation, not the camera eye rotation (which stays fixed in SS14).

---

## Architecture

| Piece | Role |
|--------|------|
| [`RTConeVisionOverlay`](../../Content.Client/_RedTruce/ConeVision/RTConeVisionOverlay.cs) | `OverlaySpace.WorldSpace`, `RequestScreenTexture`. Draws a white quad over the view with a fullscreen shader that samples `SCREEN_TEXTURE`. |
| [`RTConeVisionOverlaySystem`](../../Content.Client/_RedTruce/ConeVision/RTConeVisionOverlaySystem.cs) | Registers/removes the overlay when `rt.cone_vision.enabled` changes. |
| [`rt_cone_vision_fullscreen.swsl`](../../Resources/Textures/Shaders/rt_cone_vision_fullscreen.swsl) | Fragment shader: reconstructs **world position** from UV via **bilinear** blend of the viewport quad corners (must match `Box2Rotated` / `DrawTextureWorld`, not an axis-aligned AABB × UV). |
| [`rt_cone_vision_fullscreen.swsl`](../../Resources/Textures/Shaders/rt_cone_vision_fullscreen.swsl) | Fragment shader. |
| [`rt_cone_vision.yml`](../../Resources/Prototypes/Shaders/rt_cone_vision.yml) | Shader prototype id: `RTConeVisionFullscreen`. |
| [`CCVars.RTConeVision.cs`](../CCVar/CCVars.RTConeVision.cs) | Client CVars (`CVar.CLIENTONLY`). |

**Stacking:** The engine’s wall FOV still runs first; this pass only adds angular masking. Overlay Z-order is above shadow grain (`AfterLightTargetOverlay.ContentZIndex + 3`).

**Cone logic:** For each pixel, `worldPos` is on the ground plane; `toPixel = worldPos - playerPos`. If `length(toPixel) <= near_radius`, show unmasked. Else compare `dot(normalize(toPixel), forward)` to `cos(half_angle)`; inside cone passes through, outside becomes black (or magenta in debug).

**Forward vector:** `SharedTransformSystem.GetWorldRotation(xform).ToWorldVec()` for the local player. Origin: `GetWorldPosition(xform)`.

---

## Client CVars (defaults)

| CVar | Default | Meaning |
|------|---------|---------|
| `rt.cone_vision.enabled` | `true` | Master toggle. |
| `rt.cone_vision.half_angle_deg` | `60` | **Half** of the full cone width in degrees (default ⇒ **120°** total width). |
| `rt.cone_vision.near_radius` | `2` | World units (~tiles): always-visible disk around the player; `0` disables. |
| `rt.cone_vision.debug` | `false` | Outside cone: magenta instead of black (layout checks). |

All are `CLIENTONLY`; angle and near radius are `ARCHIVE`.

---

## Console

From the client console (F3 / dev window, depending on fork):

```text
cvar rt.cone_vision.enabled true
cvar rt.cone_vision.half_angle_deg 45
cvar rt.cone_vision.near_radius 1.5
cvar rt.cone_vision.debug true
```

Use `cvarlist rt.cone_vision` or inspect options if your fork exposes these in settings.

---

## Tuning notes

- **120° total cone:** keep `half_angle_deg` at `60` (or set `90` for a 180° hemisphere in front, etc.).
- **Tighter “immediate” footprint:** lower `near_radius`; **wider blind-spot awareness:** raise it.
- **Scientific notation** in the SWSL source is avoided (e.g. use `0.0001` not `1e-4`); some GLSL targets reject exponent literals.

---

## Shared types

[`RTConeVisionMode`](ConeVision/RTConeVisionMode.cs) — reserved for future modes (e.g. peripheral gray, sprite culling); Variant A only uses the overlay + shader path today.

---

## Future work (outline)

- **Variant B:** peripheral map tint, hide entity sprites behind the player via a separate client system; extend shader with a mode uniform or second pass.
- **Server-driven:** optional replicated component or config replication so gameplay can force cone settings without trusting client CVars alone.

---

## Launcher / ACZ

If players use a **launcher-downloaded** client, they only get this if the packaged content includes the assemblies and `Resources` above. After changing shaders or client code, rebuild the client zip and bump `build.version` so the launcher cache refreshes. For a safe local zip workflow, see [`ACZ-Cursor-Agent-Runbook.md`](ACZ-Cursor-Agent-Runbook.md).
