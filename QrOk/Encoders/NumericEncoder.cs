using QrOk.Storage;

namespace QrOk.Encoders;

internal class NumericEncoder : Encoder
{
    internal NumericEncoder(BinaryArray bitArray) : base(bitArray) { }

    private readonly Func<byte, int> DecodeToInteger = (b) => b & 0x0F;

    internal override void Encode(byte[] bytes)
    {
        foreach (var item in bytes.Chunk(3))
        {
            if (item.Length == 3) EncodeThreeCharacters(item[0], item[1], item[2]);
            else if (item.Length == 2) EncodeTwoCharacters(item[0], item[1]);
            else EncodeOneCharacter(item[0]);
        }
    }

    private void EncodeThreeCharacters(byte a, byte b, byte c)
    {
        if (a == 0) EncodeTwoCharacters(b, c);
        else
        {
            var number = DecodeToInteger(a) * 100 + DecodeToInteger(b) * 10 + DecodeToInteger(c);
            _bitArray.Append(number, 10);
        }
    }

    private void EncodeTwoCharacters(byte a, byte b)
    {
        if (a == 0) EncodeOneCharacter(b);
        else
        {
            var number = DecodeToInteger(a) * 10 + DecodeToInteger(b);
            _bitArray.Append(number, 7);
        }
    }

    private void EncodeOneCharacter(byte a)
    {
        _bitArray.Append(a, 4);
    }
}
