namespace Content.Shared._RedTruce;

/// <summary>
/// Minimal roll output for MVP dice checks.
/// Keeps individual die faces for immediate in-game debugging.
/// </summary>
public sealed class RTDiceRollResult
{
    public int Successes;
    public List<int> Faces = new();
}
