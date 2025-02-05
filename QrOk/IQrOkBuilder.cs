using QrOk.Core;
using QrOk.Enums;

namespace QrOk;

/// <summary>
/// Interface for building QR codes with configurable settings and data sources.
/// </summary>
public interface IQrOkBuilder
{
    /// <summary>
    /// Gets a new instance of the <see cref="IQrOkBuilder"/> to build a QR code.
    /// </summary>
    public static IQrOkBuilder Builder => new QrOkBuilder();

    /// <summary>
    /// Sets the width of the outline around the QR code.
    /// </summary>
    /// <param name="outlineWidth">The width of the outline.</param>
    /// <returns>The current <see cref="IQrOkBuilder"/> instance.</returns>
    public IQrOkBuilder WithOutlineWidth(int outlineWidth);

    /// <summary>
    /// Sets the error correction level for the QR code.
    /// </summary>
    /// <param name="errorCorrectionLevel">The error correction level.</param>
    /// <returns>The current <see cref="IQrOkBuilder"/> instance.</returns>
    public IQrOkBuilder WithErrorCorrectionLevel(ErrorCorrectionLevel errorCorrectionLevel);

    /// <summary>
    /// Sets the size of the generated QR code.
    /// </summary>
    /// <param name="size">The size of the QR code in pixels.</param>
    /// <returns>The current <see cref="IQrOkBuilder"/> instance.</returns>
    public IQrOkBuilder WithSize(Size size);

    /// <summary>
    /// Sets the output path for the generated QR code.
    /// </summary>
    /// <param name="path">The directory where the file will be saved.</param>
    /// <returns>The current <see cref="IQrOkBuilder"/> instance.</returns>
    public IQrOkBuilder WithOutputPath(string path);

    /// <summary>
    /// Sets the input data for the QR code from a string.
    /// </summary>
    /// <param name="message">The input string to encode in the QR code.</param>
    /// <returns>The current <see cref="IQrOkBuilder"/> instance.</returns>
    public IQrOkBuilder From(string message);

    /// <summary>
    /// Sets the input data for the QR code from a byte array.
    /// </summary>
    /// <param name="message">The byte array containing the data to encode.</param>
    /// <returns>The current <see cref="IQrOkBuilder"/> instance.</returns>
    public IQrOkBuilder From(byte[] message);

    /// <summary>
    /// Sets the input data for the QR code from a stream.
    /// </summary>
    /// <param name="message">The stream containing the data to encode.</param>
    /// <returns>The current <see cref="IQrOkBuilder"/> instance.</returns>
    public IQrOkBuilder From(Stream message);

    /// <summary>
    /// Generates the QR code and returns it as a Base64-encoded string.
    /// </summary>
    /// <returns>An <see cref="IQrOkExecutor{T}"/> that executes the generation and returns a Base64 string.</returns>
    public IQrOkExecutor<string> ToBase64();

    /// <summary>
    /// Generates the QR code and returns it as a byte array.
    /// </summary>
    /// <returns>An <see cref="IQueQrExecutor{T}"/> that executes the generation and returns a byte array.</returns>
    public IQrOkExecutor<byte[]> ToByteArray();

    /// <summary>
    /// Generates the QR code and saves it to a file.
    /// </summary>
    /// <param name="fileName">The name of the output file.</param>
    /// <returns>An <see cref="IQrOkExecutor{T}"/> that executes the generation and returns a <see cref="FileInfo"/> object.</returns>
    public IQrOkExecutor<FileInfo> ToFile(string fileName);

    /// <summary>
    /// Generates the QR code and saves it to a specified file path.
    /// </summary>
    /// <param name="fileName">The name of the output file.</param>
    /// <param name="path">The directory where the file will be saved.</param>
    /// <returns>An <see cref="IQrOkExecutor{T}"/> that executes the generation and returns a <see cref="FileInfo"/> object.</returns>
    public IQrOkExecutor<FileInfo> ToFile(string fileName, string path);
}
