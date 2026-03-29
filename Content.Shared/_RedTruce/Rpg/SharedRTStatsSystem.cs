using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce;

/// <summary>
/// Resolves effective stats/skills with species and global fallbacks; merges species defaults on map init.
/// </summary>
public sealed class SharedRTStatsSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public const string GlobalFallbackId = "RTGlobalFallback";

    private const int HardcodedFallbackStat = 10;
    private const int HardcodedFallbackSkill = 0;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RTStatsComponent, MapInitEvent>(OnStatsMapInit);
        SubscribeLocalEvent<RTSkillsComponent, MapInitEvent>(OnSkillsMapInit);
    }

    private bool TryGetSpeciesStatsDefaults(ProtoId<SpeciesPrototype> species,
        [NotNullWhen(true)] out RTSpeciesStatsDefaultsPrototype? def)
    {
        foreach (var d in _proto.EnumeratePrototypes<RTSpeciesStatsDefaultsPrototype>())
        {
            if (d.Species == species)
            {
                def = d;
                return true;
            }
        }

        def = null;
        return false;
    }

    private void OnStatsMapInit(EntityUid uid, RTStatsComponent comp, ref MapInitEvent args)
    {
        if (!TryResolveSpecies(uid, out var speciesId))
            return;

        if (!TryGetSpeciesStatsDefaults(speciesId, out var speciesDefaults))
            return;

        foreach (var (statId, val) in speciesDefaults.Stats)
        {
            if (comp.Stats.ContainsKey(statId))
                continue;
            comp.Stats[statId] = val;
        }
    }

    private void OnSkillsMapInit(EntityUid uid, RTSkillsComponent comp, ref MapInitEvent args)
    {
        if (!TryResolveSpecies(uid, out var speciesId))
            return;

        if (!TryGetSpeciesStatsDefaults(speciesId, out var speciesDefaults))
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

    public bool TryGetStat(EntityUid uid, ProtoId<RTStatPrototype> statId, out int effective, out int baseline)
    {
        if (TryComp<RTStatsComponent>(uid, out var stats) &&
            stats.Stats.TryGetValue(statId, out var v))
        {
            effective = v.Current;
            baseline = v.Baseline;
            return true;
        }

        if (TryResolveSpecies(uid, out var species) &&
            TryGetSpeciesStatsDefaults(species, out var speciesDefaults) &&
            speciesDefaults.Stats.TryGetValue(statId, out var sv))
        {
            effective = sv.Current;
            baseline = sv.Baseline;
            return true;
        }

        if (_proto.TryIndex<RTGlobalFallbackPrototype>(GlobalFallbackId, out var global) &&
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
        ProtoId<RTSkillCategoryPrototype> categoryId,
        out int effective,
        out int baseline)
    {
        if (TryComp<RTSkillsComponent>(uid, out var skills) &&
            skills.Categories.TryGetValue(categoryId, out var v))
        {
            effective = v.Current;
            baseline = v.Baseline;
            return true;
        }

        if (TryResolveSpecies(uid, out var species) &&
            TryGetSpeciesStatsDefaults(species, out var speciesDefaults) &&
            speciesDefaults.SkillCategories.TryGetValue(categoryId, out var sv))
        {
            effective = sv.Current;
            baseline = sv.Baseline;
            return true;
        }

        if (_proto.TryIndex<RTGlobalFallbackPrototype>(GlobalFallbackId, out var global) &&
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
        ProtoId<RTSkillSpecializationPrototype> specId,
        out int effective,
        out int baseline)
    {
        if (TryComp<RTSkillsComponent>(uid, out var skills) &&
            skills.Specializations.TryGetValue(specId, out var v))
        {
            effective = v.Current;
            baseline = v.Baseline;
            return true;
        }

        if (TryResolveSpecies(uid, out var species) &&
            TryGetSpeciesStatsDefaults(species, out var speciesDefaults) &&
            speciesDefaults.SkillSpecializations.TryGetValue(specId, out var sv))
        {
            effective = sv.Current;
            baseline = sv.Baseline;
            return true;
        }

        if (_proto.TryIndex<RTGlobalFallbackPrototype>(GlobalFallbackId, out var global) &&
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
    public RTStatDiceBreakdown GetSkillDiceContribution(
        EntityUid uid,
        ProtoId<RTSkillCategoryPrototype> categoryId,
        ProtoId<RTSkillSpecializationPrototype>? specializationId)
    {
        if (!_proto.TryIndex(categoryId, out RTSkillCategoryPrototype? catProto))
            return new RTStatDiceBreakdown(0, 0, 0);

        TryGetStat(uid, catProto.LinkedStat, out var statPart, out _);
        TryGetSkillCategory(uid, categoryId, out var catEff, out _);

        var specPart = 0;
        if (specializationId != null)
            TryGetSkillSpecialization(uid, specializationId.Value, out specPart, out _);

        return new RTStatDiceBreakdown(statPart, catEff, specPart);
    }

    public RTCharacterSheetData BuildCharacterSheet(EntityUid uid)
    {
        var sheet = new RTCharacterSheetData();

        var statProtos = _proto
            .EnumeratePrototypes<RTStatPrototype>()
            .OrderBy(s => s.Ordering)
            .ThenBy(s => s.ID, StringComparer.Ordinal)
            .ToList();

        foreach (var sp in statProtos)
        {
            var id = new ProtoId<RTStatPrototype>(sp.ID);
            TryGetStat(uid, id, out var effective, out var baseline);
            sheet.Stats.Add(new RTStatSheetEntry(sp.ID, baseline, effective));
        }

        int StatOrdering(ProtoId<RTStatPrototype> statId)
        {
            return _proto.TryIndex(statId, out RTStatPrototype? sp) ? sp.Ordering : 999;
        }

        var catProtos = _proto
            .EnumeratePrototypes<RTSkillCategoryPrototype>()
            .OrderBy(c => StatOrdering(c.LinkedStat))
            .ThenBy(c => c.Ordering)
            .ThenBy(c => c.ID, StringComparer.Ordinal)
            .ToList();

        foreach (var cat in catProtos)
        {
            var catId = new ProtoId<RTSkillCategoryPrototype>(cat.ID);
            TryGetSkillCategory(uid, catId, out _, out var catBase);

            var catBreakdown = GetSkillDiceContribution(uid, catId, null);
            var catPool = catBreakdown.StatPart + catBreakdown.CategoryPart + catBreakdown.SpecPart;

            var entry = new RTSkillCategorySheetEntry
            {
                CategoryId = cat.ID,
                LinkedStatId = cat.LinkedStat.Id,
                CategoryBaseline = catBase,
                CategoryEffectiveDicepool = catPool,
                DicepoolStatPart = catBreakdown.StatPart,
                DicepoolCategoryPart = catBreakdown.CategoryPart
            };

            var specs = _proto
                .EnumeratePrototypes<RTSkillSpecializationPrototype>()
                .Where(s => s.Category == catId)
                .OrderBy(s => s.Ordering)
                .ThenBy(s => s.ID, StringComparer.Ordinal);

            foreach (var spec in specs)
            {
                var specId = new ProtoId<RTSkillSpecializationPrototype>(spec.ID);
                TryGetSkillSpecialization(uid, specId, out _, out var specBase);

                var specBreakdown = GetSkillDiceContribution(uid, catId, specId);
                var specPool = specBreakdown.StatPart + specBreakdown.CategoryPart + specBreakdown.SpecPart;

                entry.Specializations.Add(new RTSkillSpecSheetEntry
                {
                    SpecId = spec.ID,
                    SpecBaseline = specBase,
                    SpecEffectiveDicepool = specPool,
                    DicepoolStatPart = specBreakdown.StatPart,
                    DicepoolCategoryPart = specBreakdown.CategoryPart,
                    DicepoolSpecPart = specBreakdown.SpecPart
                });
            }

            sheet.SkillCategories.Add(entry);
        }

        return sheet;
    }
}
