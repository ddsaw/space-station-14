using System.Collections.Generic;

namespace Content.Shared._RedTruce;

/// <summary>
/// Minimal roll request used by the MVP RedTruce dice resolver.
/// Dicetricks are intentionally deferred; <see cref="DiceTricks"/> exists as explicit future design space.
/// </summary>
public readonly record struct RTDiceRollSpec(
    int Pool,
    int DiceSides,
    int TargetNumber,
    IReadOnlyList<RTDiceTrickSpec>? DiceTricks = null
);
