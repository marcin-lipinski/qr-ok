namespace QrOk.Enums;

/// <summary>
/// Represents the four levels of error correction available for QR codes.
/// Higher levels of error correction allow the QR code to be read even if part of it is damaged or obscured.
/// </summary>
public enum ErrorCorrectionLevel
{
    /// <summary>
    /// Low (L) - Can recover up to 7% of lost data.
    /// </summary>
    L = 0,

    /// <summary>
    /// Medium (M) - Can recover up to 15% of lost data.
    /// </summary>
    M = 1,

    /// <summary>
    /// Quartile (Q) - Can recover up to 25% of lost data.
    /// </summary>
    Q = 2,

    /// <summary>
    /// High (H) - Can recover up to 30% of lost data.
    /// </summary>
    H = 3
}
