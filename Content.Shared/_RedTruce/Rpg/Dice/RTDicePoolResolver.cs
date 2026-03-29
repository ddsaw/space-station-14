using Robust.Shared.Random;

namespace Content.Shared._RedTruce;

/// <summary>
/// RedTruce dice pool resolver (MVP).
/// Rolls <see cref="RTDiceRollSpec.Pool"/> dice and counts successes where die face matches or exceeds the target number.
/// Dicetrick behavior is intentionally deferred; see TODO markers inside <see cref="Roll"/>.
/// </summary>
public static class RTDicePoolResolver
{
    public static RTDiceRollResult Roll(IRobustRandom random, RTDiceRollSpec spec)
    {
        // TODO(redtruce-dice): DICETRICK PIPELINE HOOK
        // This is the explicit future insertion point for ordered RTDiceTrickSpec processing.
        // MVP intentionally ignores spec.DiceTricks.
        _ = spec.DiceTricks;

        var pool = Math.Max(0, spec.Pool);
        var sides = Math.Max(1, spec.DiceSides);
        var result = new RTDiceRollResult();
        result.Faces.Capacity = pool;

        for (var i = 0; i < pool; i++)
        {
            var face = random.Next(1, sides + 1);
            result.Faces.Add(face);

            if (face >= spec.TargetNumber)
                result.Successes++;
        }

        return result;
    }
}
