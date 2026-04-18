using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Shared.Map;

namespace Content.Client.CombatMode;

/// <summary>
/// Draws the fantasy gauntlet cursor sprite at the mouse position as the topmost UI
/// control, so it renders above HUD widgets (buttons, panels, etc.). Hosted in the
/// <see cref="IUserInterfaceManager.PopupRoot"/> to sit on top of the regular UI.
///
/// Used only while combat mode is active to stand in for the OS cursor (which is
/// blanked out for the duration of combat). See <see cref="Content.Client.Entry.EntryPoint"/>
/// for the per-frame decision that gates <see cref="ShouldDraw"/> in sync with the
/// combat sight overlay.
/// </summary>
public sealed class GauntletHudCursorControl : Control
{
    private readonly IInputManager _input;
    private readonly Texture _texture;

    public bool ShouldDraw;

    public GauntletHudCursorControl(IInputManager input, Texture texture)
    {
        _input = input;
        _texture = texture;
        MouseFilter = MouseFilterMode.Ignore;
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        if (!ShouldDraw)
            return;

        if (_input.MouseScreenPosition.Window == WindowId.Invalid)
            return;

        handle.DrawTexture(_texture, _input.MouseScreenPosition.Position, Color.White);
    }
}
