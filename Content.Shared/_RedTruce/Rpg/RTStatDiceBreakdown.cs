namespace Content.Shared._RedTruce;

/// <summary>
/// Contribution breakdown for skill dice pools: linked stat, category rating, specialization rating.
/// </summary>
public readonly record struct RTStatDiceBreakdown(int StatPart, int CategoryPart, int SpecPart);
