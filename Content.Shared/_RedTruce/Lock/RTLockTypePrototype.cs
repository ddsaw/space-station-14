using Robust.Shared.Prototypes;

namespace Content.Shared._RedTruce.Lock;

[Prototype("rtLockType")]
public sealed partial class RTLockTypePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public LocId Name { get; private set; } = string.Empty;
}
