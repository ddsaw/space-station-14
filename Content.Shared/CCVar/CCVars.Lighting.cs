using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<bool> AmbientOcclusion =
        CVarDef.Create("light.ambient_occlusion", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Distance in world-pixels of ambient occlusion.
    /// </summary>
    public static readonly CVarDef<string> AmbientOcclusionColor =
        CVarDef.Create("light.ambient_occlusion_color", "#04080FAA", CVar.CLIENTONLY);

    /// <summary>
    /// Distance in world-pixels of ambient occlusion.
    /// </summary>
    public static readonly CVarDef<float> AmbientOcclusionDistance =
        CVarDef.Create("light.ambient_occlusion_distance", 4f, CVar.CLIENTONLY);

    /// <summary>
    /// Content-only fullscreen shadow grain strength. Set to 0 to disable.
    /// </summary>
    public static readonly CVarDef<float> LightShadowGrainStrength =
        CVarDef.Create("light.shadow_grain_strength", 0.018f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// World-space scale of the fullscreen grain pattern.
    /// </summary>
    public static readonly CVarDef<float> LightShadowGrainScale =
        CVarDef.Create("light.shadow_grain_scale", 1500000f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Luma threshold below which grain is near maximum.
    /// </summary>
    public static readonly CVarDef<float> LightShadowGrainDarkThreshold =
        CVarDef.Create("light.shadow_grain_dark_threshold", 0.45f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Luma threshold at/above which dark-based grain contribution fades to zero.
    /// Should be greater than or equal to <see cref="LightShadowGrainDarkThreshold"/>.
    /// </summary>
    public static readonly CVarDef<float> LightShadowGrainDarkThresholdMax =
        CVarDef.Create("light.shadow_grain_dark_threshold_max", 0.80f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Hard disable threshold. Above this luma grain is fully disabled.
    /// Set to 1.0 to effectively disable this cutoff.
    /// </summary>
    public static readonly CVarDef<float> LightShadowGrainDisableThreshold =
        CVarDef.Create("light.shadow_grain_disable_threshold", 0.0f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Temporal animation speed for the grain field.
    /// </summary>
    public static readonly CVarDef<float> LightShadowGrainAnimationSpeed =
        CVarDef.Create("light.shadow_grain_animation_speed", 0.0f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Extra temporal intensity modulation (0 disables flicker).
    /// </summary>
    public static readonly CVarDef<float> LightShadowGrainFlickerStrength =
        CVarDef.Create("light.shadow_grain_flicker_strength", 0.08f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Debug switch for verifying content fullscreen grain shader is active.
    /// </summary>
    public static readonly CVarDef<bool> LightShadowGrainDebug =
        CVarDef.Create("light.shadow_grain_debug", false, CVar.CLIENTONLY);
}
