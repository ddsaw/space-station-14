using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;

namespace Content.Client.Light.EntitySystems;

/// <summary>
/// Enables the content-side shadow grain overlay based on client CVars.
/// This avoids relying on the planet lighting system startup path.
/// </summary>
public sealed class ShadowGrainOverlaySystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IOverlayManager _overlay = default!;

    private readonly ShadowGrainOverlay _grain = new();
    private bool _active;

    public override void Initialize()
    {
        base.Initialize();
        _cfg.OnValueChanged(CCVars.LightShadowGrainStrength, _ => Refresh(), true);
        _cfg.OnValueChanged(CCVars.LightShadowGrainDebug, _ => Refresh(), true);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        if (_active)
            _overlay.RemoveOverlay(_grain);
        _active = false;
    }

    private void Refresh()
    {
        var enable = _cfg.GetCVar(CCVars.LightShadowGrainStrength) > 0f ||
                     _cfg.GetCVar(CCVars.LightShadowGrainDebug);

        if (enable == _active)
            return;

        if (enable)
            _overlay.AddOverlay(_grain);
        else
            _overlay.RemoveOverlay(_grain);

        _active = enable;
    }
}
