using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;

namespace Content.Client._RedTruce.ConeVision;

/// <summary>
/// Registers <see cref="RTConeVisionOverlay"/> when <c>rt.cone_vision.enabled</c> is true.
/// </summary>
public sealed class RTConeVisionOverlaySystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IOverlayManager _overlay = default!;

    private readonly RTConeVisionOverlay _cone = new();
    private bool _active;

    public override void Initialize()
    {
        base.Initialize();
        _cfg.OnValueChanged(CCVars.RTConeVisionEnabled, _ => Refresh(), true);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        if (_active)
            _overlay.RemoveOverlay(_cone);
        _active = false;
    }

    private void Refresh()
    {
        var enable = _cfg.GetCVar(CCVars.RTConeVisionEnabled);
        if (enable == _active)
            return;

        if (enable)
            _overlay.AddOverlay(_cone);
        else
            _overlay.RemoveOverlay(_cone);

        _active = enable;
    }
}
