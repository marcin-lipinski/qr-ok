using QrOk.Enums;
using QrOk.Storage;

namespace QrOk.Encoders;

internal static class EncodersFactory
{
    internal static Encoder GetEncoder(EncodingMode mode, BinaryArray bitArray)
    {
        return mode switch
        {
            EncodingMode.Numeric => new NumericEncoder(bitArray),
            EncodingMode.Alphanumeric => new AlphanumericEncoder(bitArray),
            EncodingMode.Byte => new ByteEncoder(bitArray),
            _ => new ByteEncoder(bitArray)
        };
    }
}