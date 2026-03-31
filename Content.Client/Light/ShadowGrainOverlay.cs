using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Light;

/// <summary>
/// Content-side fullscreen grain pass that targets darker regions.
/// This does not depend on engine internal light shaders.
/// </summary>
public sealed class ShadowGrainOverlay : Overlay
{
    private static readonly ProtoId<ShaderPrototype> ShaderProto = "LightShadowGrainFullscreen";

    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;

    private readonly ShaderInstance _shader;

    public ShadowGrainOverlay()
    {
        IoCManager.InjectDependencies(this);
        ZIndex = AfterLightTargetOverlay.ContentZIndex + 2;
        _shader = _prototype.Index(ShaderProto).InstanceUnique();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var strength = _cfg.GetCVar(CCVars.LightShadowGrainStrength);
        var scale = _cfg.GetCVar(CCVars.LightShadowGrainScale);
        var darkThreshold = _cfg.GetCVar(CCVars.LightShadowGrainDarkThreshold);
        var darkThresholdMax = _cfg.GetCVar(CCVars.LightShadowGrainDarkThresholdMax);
        var disableThreshold = _cfg.GetCVar(CCVars.LightShadowGrainDisableThreshold);
        var animSpeed = _cfg.GetCVar(CCVars.LightShadowGrainAnimationSpeed);
        var flickerStrength = _cfg.GetCVar(CCVars.LightShadowGrainFlickerStrength);
        var debug = _cfg.GetCVar(CCVars.LightShadowGrainDebug) ? 1f : 0f;

        if (strength <= 0f && debug < 0.5f)
            return;

        var handle = args.WorldHandle;

        // Shader-bypass probe to verify overlay execution path itself.
        if (debug > 0.5f)
        {
            handle.UseShader(null);
            handle.DrawRect(args.WorldBounds, Color.Magenta.WithAlpha(0.35f));
        }

        if (ScreenTexture == null)
            return;

        _shader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _shader.SetParameter("grainStrength", MathF.Max(0f, strength));
        _shader.SetParameter("grainScale", MathF.Max(1f, scale));
        _shader.SetParameter("darkThreshold", Math.Clamp(darkThreshold, 0.01f, 1f));
        _shader.SetParameter("darkThresholdMax", Math.Clamp(darkThresholdMax, 0.01f, 1f));
        _shader.SetParameter("disableThreshold", Math.Clamp(disableThreshold, 0.01f, 1f));
        _shader.SetParameter("animationTime", (float)_timing.RealTime.TotalSeconds);
        _shader.SetParameter("animationSpeed", MathF.Max(0f, animSpeed));
        _shader.SetParameter("flickerStrength", Math.Clamp(flickerStrength, 0f, 1f));
        var worldAabb = args.WorldBounds.CalcBoundingBox();
        _shader.SetParameter("worldMin", worldAabb.BottomLeft);
        _shader.SetParameter("worldSize", worldAabb.Size);
        _shader.SetParameter("debugMode", debug);
        handle.UseShader(_shader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }
}
