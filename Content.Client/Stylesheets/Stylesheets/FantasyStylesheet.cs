using System.Linq;
using Content.Client.ContextMenu.UI;
using Content.Client.Verbs.UI;
using Content.Client.Stylesheets.Fonts;
using Content.Client.Stylesheets.Palette;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Chat.Controls;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Utility;
using static Robust.Client.UserInterface.StylesheetHelpers;

namespace Content.Client.Stylesheets.Stylesheets;

[Virtual]
public partial class FantasyStylesheet : CommonStylesheet
{
    public override string StylesheetName => "Fantasy";

    public override NotoFontFamilyStack BaseFont { get; }

    public static readonly ResPath TextureRoot = new("/Textures/Interface/Fantasy");

    public override Dictionary<Type, ResPath[]> Roots => new()
    {
        { typeof(TextureResource), [TextureRoot, NanotrasenStylesheet.TextureRoot] },
    };

    private const int PrimaryFontSize = 12;
    private const int FontSizeStep = 2;

    // ReSharper disable once UseCollectionExpression
    private readonly List<(string?, int)> _commonFontSizes = new()
    {
        (null, PrimaryFontSize),
        (StyleClass.FontSmall, PrimaryFontSize - FontSizeStep),
        (StyleClass.FontLarge, PrimaryFontSize + FontSizeStep),
    };

    private const int PanelMargin005 = 14;
    private const int PanelMargin015 = 4;

    // Context-menu palette override. Original uses Color.DarkSlateGray (greenish) for hover.
    private static readonly ColorPalette FantasyContextButtonPalette =
        ColorPalette.FromHexBase("#1a1410") with
        {
            Element = Color.FromHex("#1a1410CC"),
            HoveredElement = Color.FromHex("#6b4c30"),
            PressedElement = Color.FromHex("#c8aa78"),
            DisabledElement = Color.FromHex("#3a2a1a"),
        };

    public FantasyStylesheet(object config, StylesheetManager man) : base(config)
    {
        BaseFont = new NotoFontFamilyStack(ResCache);

        var rules = new[]
        {
            GetRulesForFont(null, BaseFont, _commonFontSizes),
            [
                Element().Prop(Label.StylePropertyFont, BaseFont.GetFont(PrimaryFontSize)),
            ],
            GetAllSheetletRules<PalettedStylesheet, CommonSheetletAttribute>(man),
            GetAllSheetletRules<FantasyStylesheet, CommonSheetletAttribute>(man),
            GetButtonOverrideRules(),
            GetMenuButtonOverrideRules(),
            GetMarginOverrideRules(),
            GetScrollbarOverrideRules(),
            GetChatOverrideRules(),
            GetContextMenuOverrideRules(),
        };

        Stylesheet = new Stylesheet(rules.SelectMany(x => x).ToArray());
    }

    private StyleBoxTexture MakeBox(string texName, int margin, int padding = 0)
    {
        var box = new StyleBoxTexture { Texture = GetTexture(new ResPath(texName)) };
        box.SetPatchMargin(StyleBox.Margin.All, margin);
        if (padding > 0)
            box.SetPadding(StyleBox.Margin.All, padding);
        return box;
    }

    private StyleRule[] GetButtonOverrideRules()
    {
        var baseBox = MakeBox("button.svg.96dpi.png", PanelMargin015, padding: 1);
        baseBox.SetContentMarginOverride(StyleBox.Margin.Vertical, 2);
        baseBox.SetContentMarginOverride(StyleBox.Margin.Horizontal, 8);

        var smallBox = MakeBox("button_small.svg.96dpi.png", PanelMargin015);
        var roundedBorderedBox = MakeBox("rounded_button_bordered.svg.96dpi.png", PanelMargin015, padding: 2);

        return
        [
            Element<ContainerButton>()
                .Class(ContainerButton.StyleClassButton)
                .Box(baseBox),
            Element<ContainerButton>()
                .Class(ContainerButton.StyleClassButton)
                .Class(StyleClass.ButtonSmall)
                .Box(smallBox),
            Element<ContainerButton>()
                .Class(ContainerButton.StyleClassButton)
                .Class(StyleClass.ButtonOpenLeft)
                .Box(baseBox),
            Element<ContainerButton>()
                .Class(ContainerButton.StyleClassButton)
                .Class(StyleClass.ButtonOpenRight)
                .Box(baseBox),
            Element<ContainerButton>()
                .Class(ContainerButton.StyleClassButton)
                .Class(StyleClass.ButtonOpenBoth)
                .Box(baseBox),
            Element<ContainerButton>()
                .Class(ContainerButton.StyleClassButton)
                .Class(StyleClass.ButtonSquare)
                .Box(baseBox),

            // Chat filter buttons
            Element<Button>()
                .Class(ChatInputBox.StyleClassChatFilterOptionButton)
                .Box(roundedBorderedBox),
            Element<ContainerButton>()
                .Class(ChatInputBox.StyleClassChatFilterOptionButton)
                .Box(roundedBorderedBox),
        ];
    }

    // Top-right HUD buttons (character menu, construction menu, etc.).
    // MenuButtonSheetlet atlas-slices the button texture assuming a 24px-tall source.
    // Our panel-015 is 48x48 so we need to override with full-texture 9-slice boxes.
    private StyleRule[] GetMenuButtonOverrideRules()
    {
        var box = MakeBox("button.svg.96dpi.png", PanelMargin015, padding: 1);
        box.SetContentMarginOverride(StyleBox.Margin.Vertical, 2);
        box.SetContentMarginOverride(StyleBox.Margin.Horizontal, 6);

        return
        [
            Element<MenuButton>().Box(box),
            Element<MenuButton>().Class(StyleClass.ButtonSquare).Box(box),
            Element<MenuButton>().Class(StyleClass.ButtonOpenLeft).Box(box),
            Element<MenuButton>().Class(StyleClass.ButtonOpenRight).Box(box),
            Element<MenuButton>().Class(StyleClass.ButtonOpenBoth).Box(box),
            Element<MenuButton>()
                .Prop(ContainerButton.StylePropertyStyleBox, box),
            Element<MenuButton>().Class(StyleClass.ButtonOpenLeft)
                .Prop(ContainerButton.StylePropertyStyleBox, box),
            Element<MenuButton>().Class(StyleClass.ButtonOpenRight)
                .Prop(ContainerButton.StylePropertyStyleBox, box),
            Element<MenuButton>().Class(StyleClass.ButtonOpenBoth)
                .Prop(ContainerButton.StylePropertyStyleBox, box),
            Element<MenuButton>().Class(StyleClass.ButtonSquare)
                .Prop(ContainerButton.StylePropertyStyleBox, box),
        ];
    }

    private StyleRule[] GetMarginOverrideRules()
    {
        var windowBg = MakeBox("window_background.png", PanelMargin015);
        var windowBgBordered = MakeBox("window_background_bordered.png", PanelMargin005);
        var tooltipBox = MakeBox("tooltip.png", PanelMargin015);
        tooltipBox.SetContentMarginOverride(StyleBox.Margin.All, 4);
        var whisperBox = MakeBox("whisper.png", PanelMargin015);
        whisperBox.SetContentMarginOverride(StyleBox.Margin.All, 4);
        var lineEditBox = MakeBox("lineedit.png", PanelMargin015);

        var sliderFill = MakeBox("slider_fill.svg.96dpi.png", PanelMargin005);
        sliderFill.Modulate = PositivePalette.TextDark;
        var sliderBack = MakeBox("slider_fill.svg.96dpi.png", PanelMargin005);
        sliderBack.Modulate = SecondaryPalette.BackgroundDark;
        var sliderOutline = MakeBox("slider_outline.svg.96dpi.png", PanelMargin005);
        sliderOutline.Modulate = Color.FromHex("#5a4a3a");
        var sliderGrabber = MakeBox("slider_grabber.svg.96dpi.png", PanelMargin005);

        return
        [
            Element()
                .Class(DefaultWindow.StyleClassWindowPanel)
                .Panel(windowBg),
            Element()
                .Class(StyleClass.BorderedWindowPanel)
                .Panel(windowBgBordered),

            Element<PanelContainer>().Class("tooltipBox").Panel(tooltipBox),
            Element<PanelContainer>().Class("whisperBox").Panel(whisperBox),

            Element<LineEdit>().Prop(LineEdit.StylePropertyStyleBox, lineEditBox),

            Element<Slider>()
                .Prop(Slider.StylePropertyBackground, sliderBack)
                .Prop(Slider.StylePropertyForeground, sliderOutline)
                .Prop(Slider.StylePropertyGrabber, sliderGrabber)
                .Prop(Slider.StylePropertyFill, sliderFill),
        ];
    }

    private StyleRule[] GetScrollbarOverrideRules()
    {
        var grabTex = GetTexture(new ResPath("button.svg.96dpi.png"));

        var vNormal = new StyleBoxTexture { Texture = grabTex, Modulate = PrimaryPalette.Element };
        vNormal.SetPatchMargin(StyleBox.Margin.All, PanelMargin015);
        vNormal.SetContentMarginOverride(StyleBox.Margin.Left, 10);
        vNormal.SetContentMarginOverride(StyleBox.Margin.Top, 10);

        var vHover = new StyleBoxTexture(vNormal) { Modulate = PrimaryPalette.HoveredElement };
        var vGrabbed = new StyleBoxTexture(vNormal) { Modulate = PrimaryPalette.PressedElement };

        var hNormal = new StyleBoxTexture { Texture = grabTex, Modulate = PrimaryPalette.Element };
        hNormal.SetPatchMargin(StyleBox.Margin.All, PanelMargin015);
        hNormal.SetContentMarginOverride(StyleBox.Margin.Top, 10);

        var hHover = new StyleBoxTexture(hNormal) { Modulate = PrimaryPalette.HoveredElement };
        var hGrabbed = new StyleBoxTexture(hNormal) { Modulate = PrimaryPalette.PressedElement };

        return
        [
            Element<VScrollBar>().Prop(ScrollBar.StylePropertyGrabber, vNormal),
            Element<VScrollBar>().PseudoHovered().Prop(ScrollBar.StylePropertyGrabber, vHover),
            Element<VScrollBar>().PseudoPressed().Prop(ScrollBar.StylePropertyGrabber, vGrabbed),
            Element<HScrollBar>().Prop(ScrollBar.StylePropertyGrabber, hNormal),
            Element<HScrollBar>().PseudoHovered().Prop(ScrollBar.StylePropertyGrabber, hHover),
            Element<HScrollBar>().PseudoPressed().Prop(ScrollBar.StylePropertyGrabber, hGrabbed),
        ];
    }

    private StyleRule[] GetChatOverrideRules()
    {
        var chatBorderedBg = MakeBox("window_background_bordered.png", PanelMargin005);
        chatBorderedBg.Modulate = SecondaryPalette.Background.WithAlpha(221.0f / 255.0f);

        var chatOutputBg = MakeBox("window_background.png", PanelMargin015);
        chatOutputBg.Modulate = SecondaryPalette.BackgroundDark;

        return
        [
            Element<PanelContainer>()
                .Class(ChatInputBox.StyleClassChatPanel)
                .Panel(chatBorderedBg),

            Element<PanelContainer>()
                .Class(SeparatedChatGameScreen.StyleClassChatContainer)
                .Panel(chatBorderedBg),
            Element()
                .Class(SeparatedChatGameScreen.StyleClassChatContainer)
                .Panel(chatBorderedBg),

            Element<OutputPanel>()
                .Class(SeparatedChatGameScreen.StyleClassChatOutput)
                .Panel(chatOutputBg),
        ];
    }

    // Context menu: border-only popup frame, border-only items, warm-brown hover tint
    private StyleRule[] GetContextMenuOverrideRules()
    {
        // Border-only for the outer popup (transparent center so no inner flat panel)
        var popupBorder = MakeBox("border_only_ornate.png", PanelMargin005);

        // Border-only for each item row
        var itemBorder = MakeBox("border_only_simple.png", PanelMargin015);

        var rules = new List<StyleRule>
        {
            Element<PanelContainer>()
                .Class(ContextMenuPopup.StyleClassContextMenuPopup)
                .Panel(popupBorder),

            Element<ContextMenuElement>()
                .Class(ContextMenuElement.StyleClassContextMenuButton)
                .Prop(ContainerButton.StylePropertyStyleBox, itemBorder),

            Element<ContextMenuElement>()
                .Class(ConfirmationMenuElement.StyleClassConfirmationContextMenuButton)
                .Prop(ContainerButton.StylePropertyStyleBox, itemBorder),
        };

        // Replace hardcoded DarkSlateGray hover with warm brown modulation
        AddModulationRules(rules, FantasyContextButtonPalette,
            ContextMenuElement.StyleClassContextMenuButton);
        AddModulationRules(rules, NegativePalette,
            ConfirmationMenuElement.StyleClassConfirmationContextMenuButton);

        return rules.ToArray();
    }

    private static void AddModulationRules(List<StyleRule> rules, ColorPalette palette, string styleClass)
    {
        rules.Add(Element<ContextMenuElement>().Class(styleClass).PseudoNormal()
            .Prop(Control.StylePropertyModulateSelf, palette.Element));
        rules.Add(Element<ContextMenuElement>().Class(styleClass).PseudoHovered()
            .Prop(Control.StylePropertyModulateSelf, palette.HoveredElement));
        rules.Add(Element<ContextMenuElement>().Class(styleClass).PseudoPressed()
            .Prop(Control.StylePropertyModulateSelf, palette.PressedElement));
        rules.Add(Element<ContextMenuElement>().Class(styleClass).PseudoDisabled()
            .Prop(Control.StylePropertyModulateSelf, palette.DisabledElement));
    }
}
