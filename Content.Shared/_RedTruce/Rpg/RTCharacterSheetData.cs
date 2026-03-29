using Robust.Shared.Serialization;

namespace Content.Shared._RedTruce;

/// <summary>
/// Serializable snapshot for the character menu stats/skills block.
/// </summary>
[Serializable, NetSerializable]
public sealed class RTCharacterSheetData
{
    public List<RTStatSheetEntry> Stats = new();
    public List<RTSkillCategorySheetEntry> SkillCategories = new();
}

[Serializable, NetSerializable]
public readonly record struct RTStatSheetEntry(string StatId, int Baseline, int EffectiveDicepool);

[Serializable, NetSerializable]
public sealed class RTSkillCategorySheetEntry
{
    public string CategoryId = string.Empty;
    public string LinkedStatId = string.Empty;
    public int CategoryBaseline;
    public int CategoryEffectiveDicepool;
    /// <summary>Linked attribute <see cref="RTStatDiceBreakdown.StatPart"/> (effective / current) for this category row.</summary>
    public int DicepoolStatPart;
    /// <summary>Category rating <see cref="RTStatDiceBreakdown.CategoryPart"/> (effective / current).</summary>
    public int DicepoolCategoryPart;
    public List<RTSkillSpecSheetEntry> Specializations = new();
}

/// <summary>
/// One specialization row; dicepool fields mirror <see cref="SharedRTStatsSystem.GetSkillDiceContribution"/> with that spec selected.
/// </summary>
[Serializable, NetSerializable]
public sealed class RTSkillSpecSheetEntry
{
    public string? SpecId;
    public int SpecBaseline;
    public int SpecEffectiveDicepool;
    public int DicepoolStatPart;
    public int DicepoolCategoryPart;
    public int DicepoolSpecPart;
}
