using QrOk.Enums;

namespace QrOk;

/// <summary>
/// Configiure Qr code generation 
/// </summary>
public sealed class QrOkConfiguration
{
    /// <summary>
    /// Describes the size of the generated Qr code. The defualt value is M.
    /// </summary>
    public Size Size { get; set; } = Size.M;

    /// <summary>
    /// Describes the width of the outline of the Qr code in pixels. It is not part of the size. The default value is 6.
    /// </summary>
    public int OutlineWidth { get; set; } = 6;

    /// <summary>
    /// The error correction level of the Qr code. The default value is M
    /// </summary>
    public ErrorCorrectionLevel ErrorCorrectionLevel { get; set; } = ErrorCorrectionLevel.M;

    /// <summary>
    /// The path to generated file.
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;

    /// <summary>
    /// The action that converts input into bytes.
    /// </summary>
    internal Func<byte[]> GetBytesAction { get; set; } = null!;


    /// <summary>
    /// The generated file name.
    /// </summary>
    internal string FileName { get; set; } = string.Empty;

    /// <summary>
    /// The QR code version.
    /// </summary>
    internal int Version { get; set; }

    /// <summary>
    /// The QR code encoding mode.
    /// </summary>
    internal EncodingMode Mode { get; set; }
}
