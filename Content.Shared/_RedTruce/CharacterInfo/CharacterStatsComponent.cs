namespace Content.Shared._RedTruce.CharacterInfo;

/// <summary>
/// RPG-style stats shown in the character menu. Server-authoritative; the client receives values via <see cref="Content.Shared.CharacterInfo.CharacterInfoEvent"/>.
/// </summary>
[RegisterComponent]
public sealed partial class CharacterStatsComponent : Component
{
    [DataField("strength")]
    public int Strength = 12;

    [DataField("agility")]
    public int Agility = 14;

    [DataField("body")]
    public int Body = 11;

    [DataField("mind")]
    public int Mind = 15;

    [DataField("willpower")]
    public int Willpower = 9;
}
