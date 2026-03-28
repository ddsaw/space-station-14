namespace Content.Shared._RedTruce.Rpg;

[DataDefinition]
public readonly partial record struct RpgStatValue
{
    [DataField(required: true)]
    public int Baseline { get; init; }

    [DataField(required: true)]
    public int Current { get; init; }
}
