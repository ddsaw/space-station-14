using Content.Shared._RedTruce.Rpg;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._RedTruce.Character.Controls;

/// <summary>
/// RedTruce RPG stats and skills block for the character menu.
/// </summary>
public sealed class RpgCharacterSheetControl : BoxContainer
{
    public RpgCharacterSheetControl()
    {
        Orientation = LayoutOrientation.Vertical;
    }

    public void SetData(RpgCharacterSheetData data)
    {
        RemoveAllChildren();

        var statsHeading = new Label
        {
            Text = Loc.GetString("rpg-sheet-stats-heading"),
            HorizontalAlignment = HAlignment.Center,
            Margin = new Thickness(0, 0, 0, 4)
        };
        statsHeading.StyleClasses.Add("LabelHeading");
        AddChild(statsHeading);

        var statGrid = new GridContainer { Columns = 3, HSeparationOverride = 12 };
        statGrid.AddChild(MkHeader(Loc.GetString("rpg-sheet-stats-col-name")));
        statGrid.AddChild(MkHeader(Loc.GetString("rpg-sheet-stats-col-current")));
        statGrid.AddChild(MkHeader(Loc.GetString("rpg-sheet-stats-col-baseline")));

        foreach (var row in data.Stats)
        {
            var nameKey = $"rpg-stat-{row.StatId.ToLowerInvariant()}";
            var name = Loc.GetString(nameKey);
            statGrid.AddChild(new Label { Text = name, ClipText = true });
            var cur = new Label { Text = row.Current.ToString() };
            cur.StyleClasses.Add("LabelKey");
            statGrid.AddChild(cur);
            var bas = new Label
            {
                Text = row.Baseline.ToString(),
                StyleClasses = { "LabelSubText" }
            };
            statGrid.AddChild(bas);
        }

        AddChild(statGrid);

        var skillsHeading = new Label
        {
            Text = Loc.GetString("rpg-sheet-skills-heading"),
            HorizontalAlignment = HAlignment.Center,
            Margin = new Thickness(0, 10, 0, 4)
        };
        skillsHeading.StyleClasses.Add("LabelHeading");
        AddChild(skillsHeading);

        foreach (var cat in data.SkillCategories)
        {
            var catKey = $"rpg-skill-cat-{cat.CategoryId.ToLowerInvariant()}";
            var catName = Loc.GetString(catKey);
            var linkKey = $"rpg-stat-{cat.LinkedStatId.ToLowerInvariant()}";
            var linkName = Loc.GetString(linkKey);

            var catRow = new BoxContainer { Orientation = LayoutOrientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };
            var catTitle = new Label { Text = catName };
            catTitle.StyleClasses.Add("LabelKey");
            catRow.AddChild(catTitle);
            catRow.AddChild(new Label
            {
                Text = Loc.GetString("rpg-sheet-linked-stat", ("stat", linkName)),
                StyleClasses = { "LabelSubText" },
                Margin = new Thickness(8, 0, 0, 0)
            });
            AddChild(catRow);

            var catVals = new BoxContainer { Orientation = LayoutOrientation.Horizontal, Margin = new Thickness(12, 2, 0, 0) };
            var cCur = new Label { Text = cat.CategoryCurrent.ToString() };
            cCur.StyleClasses.Add("LabelKey");
            catVals.AddChild(new Label { Text = Loc.GetString("rpg-sheet-rating-current"), StyleClasses = { "LabelSubText" } });
            catVals.AddChild(cCur);
            catVals.AddChild(new Label { Text = "  ", MinWidth = 8 });
            catVals.AddChild(new Label { Text = Loc.GetString("rpg-sheet-rating-baseline"), StyleClasses = { "LabelSubText" } });
            catVals.AddChild(new Label { Text = cat.CategoryBaseline.ToString(), StyleClasses = { "LabelSubText" } });
            AddChild(catVals);

            foreach (var spec in cat.Specializations)
            {
                if (spec.SpecId == null)
                    continue;

                var specKey = $"rpg-skill-spec-{spec.SpecId.ToLowerInvariant()}";
                var specName = Loc.GetString(specKey);
                var specRow = new BoxContainer { Orientation = LayoutOrientation.Horizontal, Margin = new Thickness(24, 2, 0, 0) };
                specRow.AddChild(new Label { Text = $"• {specName}", ClipText = true });
                specRow.AddChild(new Label { Text = spec.SpecCurrent.ToString(), Margin = new Thickness(8, 0, 0, 0) });
                specRow.AddChild(new Label
                {
                    Text = $"({spec.SpecBaseline})",
                    StyleClasses = { "LabelSubText" },
                    Margin = new Thickness(6, 0, 0, 0)
                });
                AddChild(specRow);
            }
        }
    }

    private static Label MkHeader(string text)
    {
        var l = new Label { Text = text };
        l.StyleClasses.Add("LabelSubText");
        return l;
    }
}
