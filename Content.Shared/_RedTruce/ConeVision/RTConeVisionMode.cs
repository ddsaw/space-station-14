namespace Content.Shared._RedTruce.ConeVision;

/// <summary>
/// Drives client cone vision behavior. Variant A uses <see cref="BlackOutsideCone"/>.
/// Future Variant B may add peripheral map-only visibility and sprite culling.
/// </summary>
public enum RTConeVisionMode : byte
{
    Off = 0,
    BlackOutsideCone = 1,
    // PeripheralGrayHideSprites = 2, // reserved for Variant B
}
