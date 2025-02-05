using QrOk.Enums;

namespace QrOk.Helpers;

internal class BytesHelper
{
    private static readonly byte[] _encodingModeByte =
    [
        0x01, 0x02, 0x04
    ];

    private static readonly byte[] _numericBytes =
    [
        0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39
    ];

    private static readonly List<byte> _alphanumericBytes =
    [
        0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x41,
        0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C,
        0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57,
        0x58, 0x59, 0x5A, 0x20, 0x24, 0x25, 0x2A, 0x2B, 0x2D, 0x2E, 0x2F, 0x3A
    ];

    public static EncodingMode DefineMode(byte[] data)
    {
        var isNumeric = true;
        var isAlphanumeric = true;

        foreach (var item in data)
        {
            if (isNumeric && !_numericBytes.Contains(item))
            {
                isNumeric = false;
            }

            if (isAlphanumeric && !_alphanumericBytes.Contains(item))
            {
                isAlphanumeric = false;
            }

            if (!isNumeric && !isAlphanumeric)
            {
                return EncodingMode.Byte;
            }
        }

        if (isNumeric) return EncodingMode.Numeric;
        return EncodingMode.Alphanumeric;
    }

    public static byte GetModeByteRepresentation(EncodingMode mode)
    {
        return _encodingModeByte[(int)mode];
    }

    public static byte GetAlphanumericNumberRepresentation(byte alphanumeric)
    {
        return (byte)_alphanumericBytes.IndexOf(alphanumeric);
    }
}
