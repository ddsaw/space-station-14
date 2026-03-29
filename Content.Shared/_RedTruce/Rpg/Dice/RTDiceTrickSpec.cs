namespace Content.Shared._RedTruce;

/// <summary>
/// Placeholder type for future dicetrick implementation.
/// This is intentionally not executed in MVP and exists to mark the intended extension point.
/// </summary>
public readonly record struct RTDiceTrickSpec(
    RTDiceTrickKind Kind,
    int Priority = 0,
    int IntParam = 0
);

/// <summary>
/// Placeholder trick kinds for future roll modifiers.
/// Behavior is intentionally deferred.
/// </summary>
public enum RTDiceTrickKind
{
    RerollFailures,
    RerollOnFace,
    DoubleFaceSuccess,
    ExplodeOnFace,
    RerollSuccesses
}
