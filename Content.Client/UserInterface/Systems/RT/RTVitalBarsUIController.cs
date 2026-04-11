using Content.Client.Damage.Systems;
using Content.Client.Ghost;
using Content.Client.UserInterface.Systems.RT.Widgets;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using JetBrains.Annotations;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.RT;

[UsedImplicitly]
public sealed class RTVitalBarsUIController : UIController
{
    [Dependency] private readonly IPlayerManager _player = default!;

    [UISystemDependency] private readonly GhostSystem? _ghost = default;
    [UISystemDependency] private readonly MobThresholdSystem _mobThresholds = default!;
    [UISystemDependency] private readonly DamageableSystem _damageable = default!;
    [UISystemDependency] private readonly StaminaSystem _stamina = default!;

    public override void FrameUpdate(FrameEventArgs args)
    {
        var hud = UIManager.GetActiveUIWidgetOrNull<RTVitalBarsHud>();
        if (hud == null)
            return;

        hud.EnsureIconsLoaded();

        if (_player.LocalEntity is not { } player)
        {
            hud.Visible = false;
            return;
        }

        if (_ghost?.IsGhost == true)
        {
            hud.Visible = false;
            return;
        }

        hud.Visible = true;

        UpdateRTHealthBar(player, hud);
        UpdateRTStaminaBar(player, hud);
    }

    private void UpdateRTHealthBar(EntityUid player, RTVitalBarsHud hud)
    {
        if (!EntityManager.TryGetComponent<MobStateComponent>(player, out var mobState)
            || !EntityManager.TryGetComponent<MobThresholdsComponent>(player, out var thresholds)
            || !EntityManager.TryGetComponent<DamageableComponent>(player, out var damageable)
            || !_mobThresholds.TryGetIncapThreshold(player, out _, thresholds))
        {
            hud.RTHealthBar.SetAsRatio(1f);
            return;
        }

        var totalDamage = _damageable.GetTotalDamage((player, damageable));

        switch (mobState.CurrentState)
        {
            case MobState.Alive:
                if (_mobThresholds.TryGetIncapPercentage(player, totalDamage, out var incapPct, thresholds) && incapPct != null)
                    hud.RTHealthBar.SetAsRatio(1f - incapPct.Value.Float());
                else
                    hud.RTHealthBar.SetAsRatio(1f);
                break;
            case MobState.Critical:
                if (_mobThresholds.TryGetDeadPercentage(player, FixedPoint2.Max(0, totalDamage), out var deadPct, thresholds) && deadPct != null)
                    hud.RTHealthBar.SetAsRatio(1f - deadPct.Value.Float());
                else
                    hud.RTHealthBar.SetAsRatio(0f);
                break;
            case MobState.Dead:
                hud.RTHealthBar.SetAsRatio(0f);
                break;
            default:
                hud.RTHealthBar.SetAsRatio(1f);
                break;
        }
    }

    private void UpdateRTStaminaBar(EntityUid player, RTVitalBarsHud hud)
    {
        if (!EntityManager.TryGetComponent<StaminaComponent>(player, out var stam))
        {
            hud.RTStaminaRow.Visible = false;
            return;
        }

        hud.RTStaminaRow.Visible = true;
        var damage = _stamina.GetStaminaDamage(player, stam);
        var ratio = stam.CritThreshold > 0
            ? 1f - Math.Clamp(damage / stam.CritThreshold, 0f, 1f)
            : 1f;
        hud.RTStaminaBar.SetAsRatio(ratio);
    }
}
