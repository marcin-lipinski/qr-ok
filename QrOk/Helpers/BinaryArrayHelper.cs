using QrOk.Enums;
using QrOk.Storage;

namespace QrOk.Helpers;

internal static class BinaryArrayHelper
{
    public static byte[][] ToPolynomials(this BinaryArray binaryArray, ErrorCorrectionLevel level, int version)
    {
        var (groupOneBlocks, groupOneCodewordsInBlock, groupTwoBlocks, groupTwoCodewordsInBlock) = CodewordsHelper.GetCodewordsGroupInfo(level, version);
        var dataAsSpan = binaryArray.Bytes.AsSpan();
        var result = new byte[groupOneBlocks + groupTwoBlocks][];
        var offset = 0;

        for (var i = 0; i < groupOneBlocks; i++, offset += groupOneCodewordsInBlock)
        {
            result[i] = dataAsSpan.Slice(offset, groupOneCodewordsInBlock).ToArray();
        }

        for (var i = 0; i < groupTwoBlocks; i++, offset += groupTwoCodewordsInBlock)
        {
            result[i + groupOneBlocks] = dataAsSpan.Slice(offset, groupTwoCodewordsInBlock).ToArray();
        }

        return result;
    }
}
