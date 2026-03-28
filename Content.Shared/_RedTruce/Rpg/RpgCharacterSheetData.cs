using Robust.Shared.Serialization;

namespace Content.Shared._RedTruce.Rpg;

/// <summary>
/// RPG sheet snapshot sent to the client with character info.
/// </summary>
[Serializable, NetSerializable]
public sealed class RpgCharacterSheetData
{
    public List<RpgStatSheetEntry> Stats = new();
    public List<RpgSkillCategorySheetEntry> SkillCategories = new();
}

[Serializable, NetSerializable]
public readonly record struct RpgStatSheetEntry(string StatId, int Baseline, int Current);

[Serializable, NetSerializable]
public sealed class RpgSkillCategorySheetEntry
{
    public string CategoryId = "";
    public string LinkedStatId = "";
    public int CategoryBaseline;
    public int CategoryCurrent;
    public List<RpgSkillSpecSheetEntry> Specializations = new();
}

[Serializable, NetSerializable]
public readonly record struct RpgSkillSpecSheetEntry(string? SpecId, int SpecBaseline, int SpecCurrent);
