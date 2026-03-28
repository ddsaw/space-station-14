using System.Linq;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce.Rpg;

/// <summary>
/// Resolves effective RPG stats/skills with species and global fallbacks; merges species defaults on map init.
/// </summary>
public sealed class SharedRpgStatsSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public const string GlobalFallbackId = "RedTruceRpgGlobalFallback";

    private const int HardcodedFallbackStat = 10;
    private const int HardcodedFallbackSkill = 0;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RpgStatsComponent, MapInitEvent>(OnStatsMapInit);
        SubscribeLocalEvent<RpgSkillsComponent, MapInitEvent>(OnSkillsMapInit);
    }

    private void OnStatsMapInit(EntityUid uid, RpgStatsComponent comp, ref MapInitEvent args)
    {
        if (!TryResolveSpecies(uid, out var speciesId))
            return;

        if (!_proto.TryIndex<RpgSpeciesStatsDefaultsPrototype>(speciesId.Id, out var speciesDefaults))
            return;

        foreach (var (statId, val) in speciesDefaults.Stats)
        {
            if (comp.Stats.ContainsKey(statId))
                continue;
            comp.Stats[statId] = val;
        }
    }

    private void OnSkillsMapInit(EntityUid uid, RpgSkillsComponent comp, ref MapInitEvent args)
    {
        if (!TryResolveSpecies(uid, out var speciesId))
            return;

        if (!_proto.TryIndex<RpgSpeciesStatsDefaultsPrototype>(speciesId.Id, out var speciesDefaults))
            return;

        foreach (var (catId, val) in speciesDefaults.SkillCategories)
        {
            if (comp.Categories.ContainsKey(catId))
                continue;
            comp.Categories[catId] = val;
        }

        foreach (var (specId, val) in speciesDefaults.SkillSpecializations)
        {
            if (comp.Specializations.ContainsKey(specId))
                continue;
            comp.Specializations[specId] = val;
        }
    }

    public bool TryResolveSpecies(EntityUid uid, out ProtoId<SpeciesPrototype> speciesId)
    {
        speciesId = default;
        if (!TryComp<HumanoidProfileComponent>(uid, out var profile))
            return false;
        speciesId = profile.Species;
        return true;
    }

    public bool TryGetStat(EntityUid uid, ProtoId<RpgStatPrototype> statId, out int effective, out int baseline)
    {
        if (TryComp<RpgStatsComponent>(uid, out var stats) &&
            stats.Stats.TryGetValue(statId, out var v))
        {
            effective = v.Current;
            baseline = v.Baseline;
            return true;
        }

        if (TryResolveSpecies(uid, out var species) &&
            _proto.TryIndex<RpgSpeciesStatsDefaultsPrototype>(species.Id, out var speciesDefaults) &&
            speciesDefaults.Stats.TryGetValue(statId, out var sv))
        {
            effective = sv.Current;
            baseline = sv.Baseline;
            return true;
        }

        if (_proto.TryIndex<RpgGlobalFallbackPrototype>(GlobalFallbackId, out var global) &&
            global.Stats.TryGetValue(statId, out var gv))
        {
            effective = gv.Current;
            baseline = gv.Baseline;
            return true;
        }

        effective = HardcodedFallbackStat;
        baseline = HardcodedFallbackStat;
        return true;
    }

    public bool TryGetSkillCategory(
        EntityUid uid,
        ProtoId<RpgSkillCategoryPrototype> categoryId,
        out int effective,
        out int baseline)
    {
        if (TryComp<RpgSkillsComponent>(uid, out var skills) &&
            skills.Categories.TryGetValue(categoryId, out var v))
        {
            effective = v.Current;
            baseline = v.Baseline;
            return true;
        }

        if (TryResolveSpecies(uid, out var species) &&
            _proto.TryIndex<RpgSpeciesStatsDefaultsPrototype>(species.Id, out var speciesDefaults) &&
            speciesDefaults.SkillCategories.TryGetValue(categoryId, out var sv))
        {
            effective = sv.Current;
            baseline = sv.Baseline;
            return true;
        }

        if (_proto.TryIndex<RpgGlobalFallbackPrototype>(GlobalFallbackId, out var global) &&
            global.SkillCategories.TryGetValue(categoryId, out var gv))
        {
            effective = gv.Current;
            baseline = gv.Baseline;
            return true;
        }

        effective = HardcodedFallbackSkill;
        baseline = HardcodedFallbackSkill;
        return true;
    }

    public bool TryGetSkillSpecialization(
        EntityUid uid,
        ProtoId<RpgSkillSpecializationPrototype> specId,
        out int effective,
        out int baseline)
    {
        if (TryComp<RpgSkillsComponent>(uid, out var skills) &&
            skills.Specializations.TryGetValue(specId, out var v))
        {
            effective = v.Current;
            baseline = v.Baseline;
            return true;
        }

        if (TryResolveSpecies(uid, out var species) &&
            _proto.TryIndex<RpgSpeciesStatsDefaultsPrototype>(species.Id, out var speciesDefaults) &&
            speciesDefaults.SkillSpecializations.TryGetValue(specId, out var sv))
        {
            effective = sv.Current;
            baseline = sv.Baseline;
            return true;
        }

        if (_proto.TryIndex<RpgGlobalFallbackPrototype>(GlobalFallbackId, out var global) &&
            global.SkillSpecializations.TryGetValue(specId, out var gv))
        {
            effective = gv.Current;
            baseline = gv.Baseline;
            return true;
        }

        effective = HardcodedFallbackSkill;
        baseline = HardcodedFallbackSkill;
        return true;
    }

    /// <summary>
    /// Dice pool contribution for a skill use: linked stat current + category current + optional spec current.
    /// </summary>
    public RpgStatDiceBreakdown GetSkillDiceContribution(
        EntityUid uid,
        ProtoId<RpgSkillCategoryPrototype> categoryId,
        ProtoId<RpgSkillSpecializationPrototype>? specializationId)
    {
        if (!_proto.TryIndex(categoryId, out RpgSkillCategoryPrototype? catProto))
            return new RpgStatDiceBreakdown(0, 0, 0);

        TryGetStat(uid, catProto.LinkedStat, out var statPart, out _);
        TryGetSkillCategory(uid, categoryId, out var catEff, out _);

        var specPart = 0;
        if (specializationId != null)
            TryGetSkillSpecialization(uid, specializationId.Value, out specPart, out _);

        return new RpgStatDiceBreakdown(statPart, catEff, specPart);
    }

    public RpgCharacterSheetData BuildCharacterSheet(EntityUid uid)
    {
        var sheet = new RpgCharacterSheetData();

        var statProtos = _proto
            .EnumeratePrototypes<RpgStatPrototype>()
            .OrderBy(s => s.Ordering)
            .ThenBy(s => s.ID, StringComparer.Ordinal)
            .ToList();

        foreach (var sp in statProtos)
        {
            var id = new ProtoId<RpgStatPrototype>(sp.ID);
            TryGetStat(uid, id, out var cur, out var baseline);
            sheet.Stats.Add(new RpgStatSheetEntry(sp.ID, baseline, cur));
        }

        var catProtos = _proto
            .EnumeratePrototypes<RpgSkillCategoryPrototype>()
            .OrderBy(c => c.Ordering)
            .ThenBy(c => c.ID, StringComparer.Ordinal)
            .ToList();

        foreach (var cat in catProtos)
        {
            var catId = new ProtoId<RpgSkillCategoryPrototype>(cat.ID);
            TryGetSkillCategory(uid, catId, out var catCur, out var catBase);

            var entry = new RpgSkillCategorySheetEntry
            {
                CategoryId = cat.ID,
                LinkedStatId = cat.LinkedStat.Id,
                CategoryBaseline = catBase,
                CategoryCurrent = catCur
            };

            var specs = _proto
                .EnumeratePrototypes<RpgSkillSpecializationPrototype>()
                .Where(s => s.Category == catId)
                .OrderBy(s => s.Ordering)
                .ThenBy(s => s.ID, StringComparer.Ordinal);

            foreach (var spec in specs)
            {
                var specId = new ProtoId<RpgSkillSpecializationPrototype>(spec.ID);
                TryGetSkillSpecialization(uid, specId, out var specCur, out var specBase);

                if (specCur == 0 && specBase == 0)
                    continue;

                entry.Specializations.Add(new RpgSkillSpecSheetEntry(spec.ID, specBase, specCur));
            }

            sheet.SkillCategories.Add(entry);
        }

        return sheet;
    }
}
