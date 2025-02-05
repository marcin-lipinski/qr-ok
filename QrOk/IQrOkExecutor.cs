namespace QrOk;

/// <summary>
/// Interface for executing the QR code generation process and returning the result in the specified format.
/// </summary>
/// <typeparam name="T">The type of the generated QR code output (e.g., byte array, Base64 string, or file).</typeparam>

public interface IQrOkExecutor<T>
{
    /// <summary>
    /// Builds and generates the QR code, returning the result in the specified format.
    /// </summary>
    /// <returns>The generated QR code in the specified format <typeparamref name="T"/>.</returns>

    public T Build();
}
