using System.Text;

namespace QrOk.Storage;

internal class BinaryArray
{
    private readonly byte[] _internalByteArray;
    private int _currentByte = 0;
    private int _currentBit = 7;

    public BinaryArray(int size)
    {
        _internalByteArray = new byte[size];
    }

    public int Length => _internalByteArray.Length;

    public int CurrentLength => _currentByte * 8 + (7 - _currentBit);

    public byte[] Bytes => _internalByteArray;

    public void Append(int value, int length = 8)
    {
        for (var i = length - 1; i >= 0; i--)
        {
            if ((value & 1 << i) != 0x00)
            {
                _internalByteArray[_currentByte] |= (byte)(1 << _currentBit);
            }
            if (_currentBit-- == 0)
            {
                _currentBit = 7;
                _currentByte++;
            }
        }
    }

    public void MoveToNextByte()
    {
        if (_currentByte != _internalByteArray.Length && _currentBit != 7)
        {
            _currentByte++;
        }
        _currentBit = 7;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        for (var i = 0; i < Length; i++)
        {
            for (var j = 7; j >= 0; j--)
            {
                builder.Append((_internalByteArray[i] & 1 << j) != 0x00 ? '1' : '0');
            }
            builder.Append(' ');
        }
        if (builder[^1] == ' ')
        {
            builder.Remove(builder.Length - 1, 1);

        }
        return builder.ToString();
    }
}
