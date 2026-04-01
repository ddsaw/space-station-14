using System.Numerics;
using Content.Client.Light;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RedTruce.ConeVision;

/// <summary>
/// Fullscreen world overlay: black (or debug magenta) outside a forward cone centered on
/// the local player entity, aimed in the character's facing direction.
/// Runs after engine FOV. See <c>Content.Shared/_RedTruce/RTConeVision.md</c>.
/// </summary>
public sealed class RTConeVisionOverlay : Overlay
{
    private static readonly ProtoId<ShaderPrototype> ShaderProto = "RTConeVisionFullscreen";

    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IEyeManager _eyeManager = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;

    private readonly ShaderInstance _shader;

    /// <summary>
    /// Above <see cref="ShadowGrainOverlay"/> so the cone mask is the outermost pass when both run.
    /// </summary>
    public const int ZIndexValue = AfterLightTargetOverlay.ContentZIndex + 3;

    public RTConeVisionOverlay()
    {
        IoCManager.InjectDependencies(this);
        ZIndex = ZIndexValue;
        _shader = _prototype.Index(ShaderProto).InstanceUnique();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var eye = _eyeManager.CurrentEye;
        if (eye == null || eye.Position.MapId == MapId.Nullspace)
            return;

        if (ScreenTexture == null)
            return;

        var playerEnt = _player.LocalEntity;
        if (playerEnt == null || !_entManager.TryGetComponent<TransformComponent>(playerEnt.Value, out var xform))
            return;

        var xformSys = _entManager.System<SharedTransformSystem>();
        var playerWorldPos = xformSys.GetWorldPosition(xform);
        var playerWorldRot = xformSys.GetWorldRotation(xform);

        var forward = playerWorldRot.ToWorldVec();
        if (forward.LengthSquared() < 0.000001f)
            forward = new Vector2(0f, -1f);
        else
            forward = forward.Normalized();

        var halfDeg = _cfg.GetCVar(CCVars.RTConeVisionHalfAngleDeg);
        var halfRad = MathHelper.DegreesToRadians(Math.Clamp(halfDeg, 1f, 179f));
        var halfAngleCos = MathF.Cos(halfRad);

        var wb = args.WorldBounds;

        _shader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _shader.SetParameter("quadBL", wb.BottomLeft);
        _shader.SetParameter("quadBR", wb.BottomRight);
        _shader.SetParameter("quadTL", wb.TopLeft);
        _shader.SetParameter("quadTR", wb.TopRight);
        _shader.SetParameter("eyeWorldPos", playerWorldPos);
        _shader.SetParameter("eyeForward", forward);
        _shader.SetParameter("halfAngleCos", halfAngleCos);
        _shader.SetParameter("nearRadius", MathF.Max(0f, _cfg.GetCVar(CCVars.RTConeVisionNearRadius)));
        _shader.SetParameter("debugMode", _cfg.GetCVar(CCVars.RTConeVisionDebug) ? 1f : 0f);

        var handle = args.WorldHandle;
        handle.UseShader(_shader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }
}
