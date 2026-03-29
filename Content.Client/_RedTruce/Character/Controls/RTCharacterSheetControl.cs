using Content.Client.Stylesheets;
using Content.Shared._RedTruce;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RedTruce.Character.Controls;

/// <summary>
/// RedTruce stats and skills block for the character menu.
/// Display names come from prototype <see cref="RTStatPrototype.NameLocId"/> (and category/spec equivalents) so fork-prefixed IDs stay stable without duplicating Fluent keys.
/// Effective dicepool cells expose tooltips that explain the composite (baseline + future modifiers for stats; attribute + category + spec for skills).
/// </summary>
public sealed class RTCharacterSheetControl : BoxContainer
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    private static readonly Color TooltipPositiveColor = Color.FromHex("#8fdf8a");
    private static readonly Color TooltipNegativeColor = Color.FromHex("#e07070");

    private const float ColNameMin = 200f;
    private const float ColBaselineMin = 72f;
    private const float ColDicepoolMin = 118f;

    public RTCharacterSheetControl()
    {
        IoCManager.InjectDependencies(this);
        Orientation = LayoutOrientation.Vertical;
        HorizontalExpand = true;
    }

    public void SetData(RTCharacterSheetData data)
    {
        RemoveAllChildren();

        var statsHeading = new Label
        {
            Text = Loc.GetString("rpg-sheet-stats-heading"),
            HorizontalAlignment = HAlignment.Center,
            Margin = new Thickness(0, 0, 0, 6)
        };
        statsHeading.StyleClasses.Add(StyleClass.LabelHeading);
        AddChild(statsHeading);

        var statTable = new BoxContainer
        {
            Orientation = LayoutOrientation.Vertical,
            HorizontalExpand = true,
            SeparationOverride = 2
        };
        statTable.AddChild(MkHeaderRow(
            Loc.GetString("rpg-sheet-stats-col-name"),
            Loc.GetString("rpg-sheet-stats-col-baseline"),
            Loc.GetString("rpg-sheet-stats-col-dicepool")));

        foreach (var row in data.Stats)
        {
            statTable.AddChild(MkDataRow(
                StatDisplayName(row.StatId),
                row.Baseline,
                row.EffectiveDicepool,
                nameStyle: NameCellStyle.Body,
                poolTooltip: _ => DicepoolTooltipFromMessage(BuildStatDicepoolTooltip(row))));
        }

        AddChild(statTable);

        var skillsHeading = new Label
        {
            Text = Loc.GetString("rpg-sheet-skills-heading"),
            HorizontalAlignment = HAlignment.Center,
            Margin = new Thickness(0, 14, 0, 6)
        };
        skillsHeading.StyleClasses.Add(StyleClass.LabelHeading);
        AddChild(skillsHeading);

        string? prevLinkedStat = null;
        BoxContainer? skillTable = null;

        foreach (var cat in data.SkillCategories)
        {
            if (prevLinkedStat != cat.LinkedStatId)
            {
                if (skillTable != null)
                    AddChild(skillTable);

                prevLinkedStat = cat.LinkedStatId;
                var linkName = StatDisplayName(cat.LinkedStatId);

                var groupHead = new Label
                {
                    Text = Loc.GetString("rpg-sheet-skill-group", ("stat", linkName)),
                    Margin = new Thickness(0, 8, 0, 4),
                    HorizontalExpand = true
                };
                groupHead.StyleClasses.Add(StyleClass.LabelSubText);
                AddChild(groupHead);

                skillTable = new BoxContainer
                {
                    Orientation = LayoutOrientation.Vertical,
                    HorizontalExpand = true,
                    SeparationOverride = 2
                };
                skillTable.AddChild(MkHeaderRow(
                    Loc.GetString("rpg-sheet-skills-col-name"),
                    Loc.GetString("rpg-sheet-skills-col-baseline"),
                    Loc.GetString("rpg-sheet-skills-col-dicepool")));
            }

            if (skillTable == null)
                continue;

            var catName = CategoryDisplayName(cat.CategoryId);
            var statName = StatDisplayName(cat.LinkedStatId);
            skillTable.AddChild(MkDataRow(catName, cat.CategoryBaseline, cat.CategoryEffectiveDicepool,
                nameStyle: NameCellStyle.Category,
                poolTooltip: _ => DicepoolTooltipFromMessage(
                    BuildSkillCategoryDicepoolTooltip(cat.DicepoolStatPart, cat.DicepoolCategoryPart, statName, catName))));

            foreach (var spec in cat.Specializations)
            {
                if (spec.SpecId == null)
                    continue;

                var specName = SpecDisplayName(spec.SpecId);
                skillTable.AddChild(MkDataRow($"• {specName}", spec.SpecBaseline, spec.SpecEffectiveDicepool,
                    nameStyle: NameCellStyle.Spec,
                    poolTooltip: _ => DicepoolTooltipFromMessage(
                        BuildSkillSpecDicepoolTooltip(
                            spec.DicepoolStatPart,
                            spec.DicepoolCategoryPart,
                            spec.DicepoolSpecPart,
                            statName,
                            catName,
                            specName))));
            }
        }

        if (skillTable != null)
            AddChild(skillTable);
    }

    private static Tooltip DicepoolTooltipFromMessage(FormattedMessage message)
    {
        var tooltip = new Tooltip();
        tooltip.SetMessage(message);
        return tooltip;
    }

    /// <summary>Stat effective pool: baseline contribution (green) plus aggregate modifier delta when present.</summary>
    private FormattedMessage BuildStatDicepoolTooltip(RTStatSheetEntry row)
    {
        var statName = StatDisplayName(row.StatId);
        var msg = new FormattedMessage();

        AppendSignedPoolPart(msg, row.Baseline, TooltipPositiveColor, TooltipNegativeColor);
        msg.AddText(" ");
        msg.AddText(statName);
        msg.AddText(" ");
        msg.AddText(Loc.GetString("rpg-sheet-tooltip-stat-baseline-suffix"));

        var delta = row.EffectiveDicepool - row.Baseline;
        msg.AddText("\n");
        if (delta == 0)
        {
            msg.AddText(Loc.GetString("rpg-sheet-tooltip-stat-no-extra"));
            return msg;
        }

        AppendSignedPoolPart(msg, delta, TooltipPositiveColor, TooltipNegativeColor);
        msg.AddText(" ");
        msg.AddText(Loc.GetString("rpg-sheet-tooltip-stat-modifiers-suffix"));
        return msg;
    }

    private FormattedMessage BuildSkillCategoryDicepoolTooltip(
        int statPart,
        int categoryPart,
        string statDisplayName,
        string categoryDisplayName)
    {
        var msg = new FormattedMessage();
        AppendSkillTerm(msg, statPart, statDisplayName);
        msg.AddText(" + ");
        AppendSkillTerm(msg, categoryPart, categoryDisplayName);
        msg.AddText("\n");
        msg.AddText(Loc.GetString("rpg-sheet-tooltip-skill-composite-hint"));
        return msg;
    }

    private FormattedMessage BuildSkillSpecDicepoolTooltip(
        int statPart,
        int categoryPart,
        int specPart,
        string statDisplayName,
        string categoryDisplayName,
        string specDisplayName)
    {
        var msg = new FormattedMessage();
        AppendSkillTerm(msg, statPart, statDisplayName);
        msg.AddText(" + ");
        AppendSkillTerm(msg, categoryPart, categoryDisplayName);
        msg.AddText(" + ");
        AppendSkillTerm(msg, specPart, specDisplayName);
        msg.AddText("\n");
        msg.AddText(Loc.GetString("rpg-sheet-tooltip-skill-composite-hint"));
        return msg;
    }

    private static void AppendSkillTerm(FormattedMessage msg, int value, string displayName)
    {
        AppendSignedPoolPart(msg, value, TooltipPositiveColor, TooltipNegativeColor);
        msg.AddText($" ({displayName})");
    }

    /// <summary>Formats a contribution with +N for non-negative N (green); negative values show sign in red.</summary>
    private static void AppendSignedPoolPart(FormattedMessage msg, int value, Color positiveColor, Color negativeColor)
    {
        if (value >= 0)
        {
            msg.PushColor(positiveColor);
            msg.AddText($"+{value}");
            msg.Pop();
        }
        else
        {
            msg.PushColor(negativeColor);
            msg.AddText(value.ToString());
            msg.Pop();
        }
    }

    private string StatDisplayName(string statId) =>
        _proto.TryIndex<RTStatPrototype>(statId, out var p) ? Loc.GetString(p.NameLocId) : statId;

    private string CategoryDisplayName(string categoryId) =>
        _proto.TryIndex<RTSkillCategoryPrototype>(categoryId, out var p) ? Loc.GetString(p.NameLocId) : categoryId;

    private string SpecDisplayName(string specId) =>
        _proto.TryIndex<RTSkillSpecializationPrototype>(specId, out var p) ? Loc.GetString(p.NameLocId) : specId;

    private enum NameCellStyle
    {
        Body,
        Category,
        Spec
    }

    private static BoxContainer MkHeaderRow(string name, string baseline, string dicepool)
    {
        var row = NewRow();

        var nameL = MkHeaderLabel(name, HAlignment.Left);
        nameL.MinWidth = ColNameMin;
        nameL.HorizontalExpand = true;

        var baseL = MkHeaderLabel(baseline, HAlignment.Right);
        baseL.MinWidth = ColBaselineMin;

        var poolL = MkHeaderLabel(dicepool, HAlignment.Right);
        poolL.MinWidth = ColDicepoolMin;

        row.AddChild(nameL);
        row.AddChild(baseL);
        row.AddChild(poolL);
        return row;
    }

    private static Label MkHeaderLabel(string text, HAlignment hAlign)
    {
        var l = new Label
        {
            Text = text,
            ClipText = false,
            HorizontalAlignment = hAlign,
            VerticalAlignment = VAlignment.Center,
            Margin = new Thickness(0, 0, 0, 4)
        };
        l.StyleClasses.Add(StyleClass.LabelSubText);
        return l;
    }

    private static BoxContainer MkDataRow(
        string nameText,
        int baseline,
        int dicepool,
        NameCellStyle nameStyle,
        TooltipSupplier? poolTooltip = null)
    {
        var row = NewRow();

        var name = new Label
        {
            Text = nameText,
            ClipText = false,
            HorizontalAlignment = HAlignment.Left,
            VerticalAlignment = VAlignment.Center,
            HorizontalExpand = true,
            MinWidth = ColNameMin
        };

        switch (nameStyle)
        {
            case NameCellStyle.Category:
                name.StyleClasses.Add(StyleClass.LabelKeyText);
                break;
            case NameCellStyle.Spec:
                name.Margin = new Thickness(12, 0, 0, 0);
                break;
            case NameCellStyle.Body:
                break;
        }

        var baseL = new Label
        {
            Text = baseline.ToString(),
            ClipText = false,
            HorizontalAlignment = HAlignment.Right,
            VerticalAlignment = VAlignment.Center,
            MinWidth = ColBaselineMin
        };
        baseL.StyleClasses.Add(StyleClass.LabelWeak);

        var poolL = new Label
        {
            Text = dicepool.ToString(),
            ClipText = false,
            HorizontalAlignment = HAlignment.Right,
            VerticalAlignment = VAlignment.Center,
            MinWidth = ColDicepoolMin
        };
        poolL.StyleClasses.Add(StyleClass.Highlight);

        if (poolTooltip != null)
        {
            poolL.TooltipSupplier = poolTooltip;
            poolL.MouseFilter = MouseFilterMode.Stop;
        }

        row.AddChild(name);
        row.AddChild(baseL);
        row.AddChild(poolL);
        return row;
    }

    private static BoxContainer NewRow()
    {
        return new BoxContainer
        {
            Orientation = LayoutOrientation.Horizontal,
            HorizontalExpand = true,
            SeparationOverride = 14
        };
    }
}
