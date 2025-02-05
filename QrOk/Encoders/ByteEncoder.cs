using QrOk.Storage;

namespace QrOk.Encoders;

internal class ByteEncoder : Encoder
{
    internal ByteEncoder(BinaryArray bitArray) : base(bitArray) { }

    internal override void Encode(byte[] bytes)
    {
        foreach (var item in bytes)
        {
            _bitArray.Append(item);
        }
    }
}
