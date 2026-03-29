namespace Content.Shared._RedTruce;

[DataDefinition]
public readonly partial record struct RTStatValue
{
    [DataField(required: true)]
    public int Baseline { get; init; }

    [DataField(required: true)]
    public int Current { get; init; }
}
