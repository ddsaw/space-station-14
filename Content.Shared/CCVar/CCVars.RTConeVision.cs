using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Enables the RedTruce content-only forward cone mask (black outside the cone).
    /// </summary>
    public static readonly CVarDef<bool> RTConeVisionEnabled =
        CVarDef.Create("rt.cone_vision.enabled", true, CVar.CLIENTONLY);

    /// <summary>
    /// Half-angle of the forward vision cone in degrees. Full cone width is <c>2 *</c> this value.
    /// </summary>
    public static readonly CVarDef<float> RTConeVisionHalfAngleDeg =
        CVarDef.Create("rt.cone_vision.half_angle_deg", 60f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// Radius (in world units / tiles) around the player that is always visible regardless of cone.
    /// Set to 0 to disable the near circle.
    /// </summary>
    public static readonly CVarDef<float> RTConeVisionNearRadius =
        CVarDef.Create("rt.cone_vision.near_radius", 2f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /// <summary>
    /// When true, pixels outside the cone are tinted magenta instead of black (debug).
    /// </summary>
    public static readonly CVarDef<bool> RTConeVisionDebug =
        CVarDef.Create("rt.cone_vision.debug", false, CVar.CLIENTONLY);
}
