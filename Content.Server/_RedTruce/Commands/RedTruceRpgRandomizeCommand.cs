using Content.Server.Administration;
using Content.Shared._RedTruce;
using Content.Shared.Administration;
using JetBrains.Annotations;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._RedTruce.Commands;

/// <summary>
/// Debug aid: randomize RPG stats/skills on your mob so the character menu dicepool math can be checked visually.
/// </summary>
[UsedImplicitly]
[AdminCommand(AdminFlags.Debug)]
public sealed class RedTruceRpgRandomizeCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _ent = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public string Command => "rpg_randomize";

    public string Description => Loc.GetString("cmd-rpg-randomize-desc");

    public string Help => Loc.GetString("cmd-rpg-randomize-help");

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { AttachedEntity: { } uid })
        {
            shell.WriteError(Loc.GetString("cmd-rpg-randomize-no-player"));
            return;
        }

        if (!_ent.TryGetComponent(uid, out RTStatsComponent? stats) ||
            !_ent.TryGetComponent(uid, out RTSkillsComponent? skills))
        {
            shell.WriteError(Loc.GetString("cmd-rpg-randomize-missing"));
            return;
        }

        // Stats: randomize baseline only; effective (Current) is set equal so rolls use baseline until
        // future systems (curses, buffs) change Current independently.
        stats.Stats.Clear();
        foreach (var sp in _proto.EnumeratePrototypes<RTStatPrototype>())
        {
            var id = new ProtoId<RTStatPrototype>(sp.ID);
            var baseline = _random.Next(6, 19);
            stats.Stats[id] = new RTStatValue { Baseline = baseline, Current = baseline };
        }

        skills.Categories.Clear();
        foreach (var cat in _proto.EnumeratePrototypes<RTSkillCategoryPrototype>())
        {
            var id = new ProtoId<RTSkillCategoryPrototype>(cat.ID);
            var rating = _random.Next(0, 4);
            skills.Categories[id] = new RTStatValue { Baseline = rating, Current = rating };
        }

        skills.Specializations.Clear();
        foreach (var spec in _proto.EnumeratePrototypes<RTSkillSpecializationPrototype>())
        {
            var id = new ProtoId<RTSkillSpecializationPrototype>(spec.ID);
            var rating = _random.Next(0, 4);
            skills.Specializations[id] = new RTStatValue { Baseline = rating, Current = rating };
        }

        // RTStats/RTSkills are not networked; the client sees values via CharacterInfoEvent when opening the menu.

        shell.WriteLine(Loc.GetString("cmd-rpg-randomize-done"));
    }
}
