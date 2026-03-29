using Content.Server.Administration;
using Content.Shared._RedTruce;
using Content.Shared.Administration;
using JetBrains.Annotations;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._RedTruce.Commands;

/// <summary>
/// Debug aid: roll a skill's effective dicepool against a passed TN.
/// </summary>
[UsedImplicitly]
[AdminCommand(AdminFlags.Debug)]
public sealed class RedTruceRpgRollSkillCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _ent = default!;
    [Dependency] private readonly IEntitySystemManager _sys = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public string Command => "rpg_roll_skill";
    public string Description => Loc.GetString("cmd-rpg-roll-skill-desc");
    public string Help => Loc.GetString("cmd-rpg-roll-skill-help");

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { AttachedEntity: { } uid })
        {
            shell.WriteError(Loc.GetString("cmd-rpg-roll-skill-no-player"));
            return;
        }

        if (args.Length is not 2 and not 3)
        {
            shell.WriteError(Loc.GetString("cmd-rpg-roll-skill-invalid-args"));
            return;
        }

        var categoryRaw = args[0];
        string? specRaw = null;
        var tnRaw = args[^1];
        if (args.Length == 3)
            specRaw = args[1];

        if (!int.TryParse(tnRaw, out var tn))
        {
            shell.WriteError(Loc.GetString("cmd-rpg-roll-skill-invalid-tn", ("tn", tnRaw)));
            return;
        }

        if (!_proto.TryIndex<RTSkillCategoryPrototype>(categoryRaw, out var _))
        {
            shell.WriteError(Loc.GetString("cmd-rpg-roll-skill-invalid-category", ("category", categoryRaw)));
            return;
        }

        var categoryId = new ProtoId<RTSkillCategoryPrototype>(categoryRaw);
        ProtoId<RTSkillSpecializationPrototype>? specId = null;
        if (!string.IsNullOrWhiteSpace(specRaw) && specRaw != "-" && specRaw != "none")
        {
            if (!_proto.TryIndex<RTSkillSpecializationPrototype>(specRaw, out var _))
            {
                shell.WriteError(Loc.GetString("cmd-rpg-roll-skill-invalid-spec", ("spec", specRaw)));
                return;
            }

            specId = new ProtoId<RTSkillSpecializationPrototype>(specRaw);
        }

        if (!_ent.HasComponent<RTSkillsComponent>(uid) || !_ent.HasComponent<RTStatsComponent>(uid))
        {
            shell.WriteError(Loc.GetString("cmd-rpg-roll-skill-missing"));
            return;
        }

        var stats = _sys.GetEntitySystem<SharedRTStatsSystem>();
        var parts = stats.GetSkillDiceContribution(uid, categoryId, specId);
        var pool = parts.StatPart + parts.CategoryPart + parts.SpecPart;

        // TODO(redtruce-dice): PRE-ROLL SITUATIONAL MODIFIER GATHER HOOK
        // Future step: gather ultra short-lived context (darkness, cover posture, transient stance)
        // and adjust pool / TN before creating RTDiceRollSpec.

        var spec = new RTDiceRollSpec(pool, 10, tn);
        var roll = RTDicePoolResolver.Roll(_random, spec);
        var specLabel = specId?.Id ?? "none";
        var faces = roll.Faces.Count == 0 ? "-" : string.Join(", ", roll.Faces);

        shell.WriteLine(Loc.GetString("cmd-rpg-roll-skill-result",
            ("category", categoryRaw),
            ("spec", specLabel),
            ("pool", pool),
            ("tn", tn),
            ("successes", roll.Successes),
            ("faces", faces)));
    }
}
