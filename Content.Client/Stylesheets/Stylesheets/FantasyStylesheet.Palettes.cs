using Content.Client.Stylesheets.Palette;

namespace Content.Client.Stylesheets.Stylesheets;

public partial class FantasyStylesheet
{
    public static readonly ColorPalette Leather = ColorPalette.FromHexBase("#453e38", lightnessShift: 0.06f, chromaShift: 0.005f);
    public static readonly ColorPalette Parchment = ColorPalette.FromHexBase("#5a4a3a", lightnessShift: 0.05f, chromaShift: 0.003f);
    public static readonly ColorPalette ForestGreen = ColorPalette.FromHexBase("#3a6b3a", chromaShift: 0.02f);
    public static readonly ColorPalette BloodRed = ColorPalette.FromHexBase("#8b2020", chromaShift: 0.02f);
    public static readonly ColorPalette AntiquGold = ColorPalette.FromHexBase("#b8922e");

    public override ColorPalette PrimaryPalette => Leather;
    public override ColorPalette SecondaryPalette => Parchment;
    public override ColorPalette PositivePalette => ForestGreen;
    public override ColorPalette NegativePalette => BloodRed;
    public override ColorPalette HighlightPalette => AntiquGold;
}
