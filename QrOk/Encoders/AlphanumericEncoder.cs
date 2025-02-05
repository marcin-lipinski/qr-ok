using QrOk.Helpers;
using QrOk.Storage;

namespace QrOk.Encoders;

internal class AlphanumericEncoder : Encoder
{
    internal AlphanumericEncoder(BinaryArray bitArray) : base(bitArray) { }

    internal override void Encode(byte[] bytes)
    {
        for (var i = 1; i <= bytes.Length; i++)
        {
            if (i % 2 == 0)
            {
                var a = BytesHelper.GetAlphanumericNumberRepresentation(bytes[i - 2]);
                var b = BytesHelper.GetAlphanumericNumberRepresentation(bytes[i - 1]);
                var ab = a * 45 + b;
                _bitArray.Append(ab, 11);
            }
        }

        if (bytes.Length % 2 == 1)
        {
            var a = BytesHelper.GetAlphanumericNumberRepresentation(bytes[^1]);
            _bitArray.Append(a, 6);
        }
    }
}
