using QrOk.Enums;
using System.Collections;
using System.IO.Compression;

namespace QrOk.Generators;

internal static class PNGFileGenerator
{
    public static MemoryStream GetPNGStream(BitArray[] matrix, Size size)
    {
        var imageSize = CalculateOutputSize(size, matrix.Length);
        var scaledPixelData = ScalePixelData(matrix, matrix.Length, imageSize);

        var memoryStream = new MemoryStream();
        var writer = new BinaryWriter(memoryStream);

        writer.Write([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);

        WriteChunk(writer, "IHDR", GetIHDRChunkData(imageSize, imageSize));
        var imageData = GetImageData(scaledPixelData, imageSize);
        var compressedData = Compress(imageData);
        WriteChunk(writer, "IDAT", compressedData);
        WriteChunk(writer, "IEND", []);

        return memoryStream;
    }

    internal static FileInfo SaveToFile(this MemoryStream stream, string fileName, string outputPath)
    {
        if (!fileName.EndsWith(".png"))
        {
            fileName += ".png";
        }

        var fullPath = Path.Combine(outputPath, fileName);

        using var fileStream = new FileStream(fullPath, FileMode.Create);
        stream.WriteTo(fileStream);

        return new FileInfo(outputPath);
    }

    private static int CalculateOutputSize(Size size, int originalSize)
    {
        if (size == Size.Original) return originalSize;

        var newSize = originalSize;
        while (newSize < (int)size - originalSize)
        {
            newSize += originalSize;
        }
        return newSize;
    }

    private static bool[] ScalePixelData(BitArray[] matrix, int originalSize, int outputSize)
    {
        var inputSize = matrix.Length;
        var scaledPixelData = new List<bool>();
        var scale = outputSize / originalSize;

        var parsed = new bool[inputSize * inputSize];
        for (var i = 0; i < inputSize; i++)
        {
            matrix[i].CopyTo(parsed, i * inputSize);
        }

        for (int y = 0; y < outputSize; y++)
        {
            for (int x = 0; x < outputSize; x++)
            {
                int origX = x / scale;
                int origY = y / scale;
                int index = origY * inputSize + origX;

                scaledPixelData.Add(parsed[index]);
            }
        }

        return [.. scaledPixelData];
    }

    private static byte[] GetIHDRChunkData(int width, int height)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(ToBigEndian(width));
        writer.Write(ToBigEndian(height));
        writer.Write((byte)0x08);
        writer.Write((byte)0x00);
        writer.Write((byte)0x00);
        writer.Write((byte)0x00);
        writer.Write((byte)0x00);

        return ms.ToArray();
    }

    private static byte[] GetImageData(bool[] pixelData, int imageSize)
    {
        var imageData = new List<byte>();

        for (int row = 0; row < imageSize; row++)
        {
            imageData.Add(0x00);
            for (int col = 0; col < imageSize; col++)
            {
                imageData.Add(pixelData[row * imageSize + col] ? (byte)0x00 : (byte)0xFF);
            }
        }

        return [.. imageData];
    }

    private static byte[] Compress(byte[] data)
    {
        using var ms = new MemoryStream();
        using var compressor = new DeflateStream(ms, CompressionLevel.Optimal, true);

        compressor.Write(data, 0, data.Length);
        compressor.Close();

        return ms.ToArray();
    }

    private static void WriteChunk(BinaryWriter writer, string type, byte[] data)
    {
        writer.Write(ToBigEndian(data.Length));
        writer.Write(type.ToCharArray());
        writer.Write(data);
        writer.Write(ToBigEndian(CalculateCrc(type, data)));
    }

    private static uint CalculateCrc(string type, byte[] data)
    {
        uint crc = 0xFFFFFFFF;
        foreach (var b in type.ToCharArray())
        {
            crc = UpdateCrc(crc, (byte)b);
        }
        foreach (var b in data)
        {
            crc = UpdateCrc(crc, b);
        }
        return ~crc;
    }

    private static uint UpdateCrc(uint crc, byte b)
    {
        uint[] table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            uint c = i;
            for (int j = 0; j < 8; j++)
            {
                c = (c & 1) != 0 ? 0xedb88320 ^ c >> 1 : c >> 1;
            }
            table[i] = c;
        }
        return table[(crc ^ b) & 0xFF] ^ crc >> 8;
    }

    private static byte[] ToBigEndian(int value)
    {
        return BitConverter.GetBytes(value).Reverse().ToArray();
    }

    private static byte[] ToBigEndian(uint value)
    {
        return BitConverter.GetBytes(value).Reverse().ToArray();
    }
}
