using QrOk.Storage;

namespace QrOk.Encoders;

internal abstract class Encoder
{
    protected readonly BinaryArray _bitArray;

    protected Encoder(BinaryArray bitArray)
    {
        _bitArray = bitArray;
    }

    internal abstract void Encode(byte[] data);
}