using QrOk.Helpers;
using QrOk.Storage;

namespace QrOk.Core;

internal class BinaryMatrixBuilder
{
    private readonly int _size;
    private readonly QrOkConfiguration _configuration;
    private readonly BinaryMatrix _reservationMatrix = null!;
    private readonly BinaryMatrix[] _maskingMatrices = null!;
    private readonly List<(int x, int y)> _reservedAlignmentAreas = [];
    private int _commonDarkModulesCount = 0;

    public BinaryMatrixBuilder(QrOkConfiguration configuration)
    {
        _configuration = configuration;
        _size = (configuration.Version - 1) * 4 + 21;
        _reservationMatrix = new BinaryMatrix(_size, (row, column) => false);

        _maskingMatrices = new BinaryMatrix[8];
        for (int i = 0; i < _maskingMatrices.Length; i++)
        {
            _maskingMatrices[i] = new BinaryMatrix(_size, BinaryMatrixHelper.MaskingFunctions[i]);
        }
    }

    public void PreMaskingActions()
    {
        ReserveFindersPatternsWithSeperators();
        ReserveAlignmentPattern();
        ReserveTimingPatterns();
        ReserveFormatAreas();
        ReserveVersionAreas();
        AddDarkModule();
    }

    public BinaryMatrix GetBestMatrixWithOutline()
    {
        var bestPenaltyScore = int.MaxValue;
        var bestPenaltyIndex = 0;
        for (var i = 0; i < 8; i++)
        {
            AddFindersPatternsWithSeperators(_maskingMatrices[i]);
            AddAlignmentPattern(_maskingMatrices[i]);
            AddTimingPatterns(_maskingMatrices[i]);
            var score = _maskingMatrices[i].CountScore(_commonDarkModulesCount);
            if (score < bestPenaltyScore)
            {
                bestPenaltyIndex = i;
                bestPenaltyScore = score;
            }
        }
        AddFormatAreas(_maskingMatrices[bestPenaltyIndex], bestPenaltyIndex);
        AddVersionAreas(_maskingMatrices[bestPenaltyIndex]);
        _maskingMatrices[bestPenaltyIndex].AddOutline(_configuration.OutlineWidth);

        return _maskingMatrices[bestPenaltyIndex];
    }

    private void ReserveFindersPatternsWithSeperators()
    {
        ReadOnlySpan<byte> area = [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];
        PlacePattern(_reservationMatrix, area, 0, 0);
        PlacePattern(_reservationMatrix, area, 0, _size - 8);
        PlacePattern(_reservationMatrix, area, _size - 8, 0);

        _commonDarkModulesCount += 99;
    }

    private void ReserveAlignmentPattern()
    {
        if (_configuration.Version == 1) return;

        ReadOnlySpan<byte> pattern = [0x1F, 0x1F, 0x1F, 0x1F, 0x1F];

        var coordinates = BinaryMatrixHelper.GetAlignementPatternsCoordinates(_configuration.Version);
        int[][] findersPatternsEdgeCoords =
        [
            [7, 7],
            [7, _size - 7],
            [_size - 7, 7]
        ];

        foreach (var x in coordinates)
        {
            foreach (var y in coordinates)
            {
                if (IsOverlappingFinderPattern(x, y, findersPatternsEdgeCoords)) continue;

                _reservedAlignmentAreas.Add((x, y));
                _commonDarkModulesCount += 17;
                for (var i = 0; i < pattern.Length; i++)
                {
                    PlaceBites(_reservationMatrix, pattern[i], x - 2 + i, y - 2, 5);
                }
            }
        }
    }

    private void ReserveTimingPatterns()
    {
        for (int i = 8; i < _size - 8; i++)
        {
            _reservationMatrix.Matrix[6][i] = true;
            _reservationMatrix.Matrix[i][6] = true;

            if (i % 2 == 0) _commonDarkModulesCount += 2;
        }
    }

    private void ReserveFormatAreas()
    {
        for (var i = 0; i < 9; i++)
        {
            _reservationMatrix.Matrix[i][8] = true;
            _reservationMatrix.Matrix[8][i] = true;
        }
        for (var i = _size - 8; i < _size; i++)
        {
            _reservationMatrix.Matrix[8][i] = true;
            _reservationMatrix.Matrix[i][8] = true;
        }
    }

    private void ReserveVersionAreas()
    {
        if (_configuration.Version < 7) return;

        for (var i = 0; i < 6; i++)
        {
            for (var j = _size - 11; j < _size - 8; j++)
            {
                _reservationMatrix.Matrix[i][j] = true;
                _reservationMatrix.Matrix[j][i] = true;
            }
        }
    }

    private void AddDarkModule()
    {
        _reservationMatrix.Matrix[_size - 8][8] = true;
        _commonDarkModulesCount++;

        foreach (var matrix in _maskingMatrices)
        {
            matrix.Matrix[_size - 8][8] = true;
        }
    }

    public void MaskData(Span<byte> data)
    {
        var totalBits = data.Length * 8;
        var bitIndex = 0;
        var colIndex = _size - 1;
        var rowIndex = _size - 1;
        var upward = true;

        while (colIndex > 0)
        {
            if (colIndex == 6) colIndex--;

            for (var i = 0; i < _size; i++)
            {
                var currRow = upward ? rowIndex - i : rowIndex + i;
                for (var j = 0; j < 2; j++)
                {
                    var currCol = colIndex - j;
                    if (!_reservationMatrix.Matrix[currRow][currCol])
                    {
                        var bit = false;
                        if (bitIndex < totalBits)
                        {
                            var byteIndex = bitIndex / 8;
                            var shift = 7 - bitIndex % 8;
                            bit = (data[byteIndex] & 1 << shift) != 0;
                        }

                        foreach (var matrix in _maskingMatrices)
                        {
                            matrix.Mask(currRow, currCol, bit);
                        }
                        bitIndex++;
                    }
                }
            }
            upward = !upward;

            if (upward)
            {
                rowIndex = _size - 1;
            }
            else
            {
                rowIndex = 0;
            }
            colIndex -= 2;
        }
    }

    private void AddFindersPatternsWithSeperators(BinaryMatrix currentMatrix)
    {
        ReadOnlySpan<byte> topLeft = [0xFE, 0x82, 0xBA, 0xBA, 0xBA, 0x82, 0xFE, 0x00];
        ReadOnlySpan<byte> topRight = [0x7F, 0x41, 0x5D, 0x5D, 0x5D, 0x41, 0x7F, 0x00];
        ReadOnlySpan<byte> bottomLeft = [0x00, 0xFE, 0x82, 0xBA, 0xBA, 0xBA, 0x82, 0xFE];

        PlacePattern(currentMatrix, topLeft, 0, 0);
        PlacePattern(currentMatrix, topRight, 0, (short)(_size - 8));
        PlacePattern(currentMatrix, bottomLeft, (short)(_size - 8), 0);
    }

    private void AddAlignmentPattern(BinaryMatrix currentMatrix)
    {
        if (_configuration.Version == 1) return;

        ReadOnlySpan<byte> pattern = [0x1F, 0x11, 0x15, 0x11, 0x1F];
        foreach (var (x, y) in _reservedAlignmentAreas)
        {
            for (var i = 0; i < pattern.Length; i++)
            {
                PlaceBites(currentMatrix, pattern[i], x - 2 + i, y - 2, 5);
            }
        }
    }

    private void AddTimingPatterns(BinaryMatrix currentMatrix)
    {
        for (int i = 8; i < _size - 8; i++)
        {
            currentMatrix.Matrix[6][i] = i % 2 == 0;
            currentMatrix.Matrix[i][6] = i % 2 == 0;
        }
    }

    private void AddFormatAreas(BinaryMatrix currentMatrix, int maskVerison)
    {
        var formatString = BinaryMatrixHelper.GetFormatString(_configuration.ErrorCorrectionLevel, maskVerison);

        PlaceBites(currentMatrix, (byte)(formatString >> 9), 8, 0, 6);
        PlaceBites(currentMatrix, (byte)formatString, 8, _size - 8, 8);

        PlaceBites(currentMatrix, (byte)(formatString >> 0 & 0x01), 0, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 1 & 0x01), 1, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 2 & 0x01), 2, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 3 & 0x01), 3, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 4 & 0x01), 4, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 5 & 0x01), 5, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 6 & 0x01), 7, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 7 & 0x01), 8, 8, 1);

        PlaceBites(currentMatrix, (byte)(formatString >> 8 & 0x01), 8, 7, 1);

        PlaceBites(currentMatrix, (byte)(formatString >> 8 & 0x01), _size - 7, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 9 & 0x01), _size - 6, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 10 & 0x01), _size - 5, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 11 & 0x01), _size - 4, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 12 & 0x01), _size - 3, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 13 & 0x01), _size - 2, 8, 1);
        PlaceBites(currentMatrix, (byte)(formatString >> 14 & 0x01), _size - 1, 8, 1);
    }

    private void AddVersionAreas(BinaryMatrix currentMatrix)
    {
        if (_configuration.Version < 7) return;

        var versionString = BinaryMatrixHelper.GetVersionString(_configuration.Version);

        for (int i = 0; i <= 2; i++)
        {
            for (var j = 0; j < 6; j++)
            {
                var value = (versionString >> 3 * j + i & 0x01) != 0x00;
                currentMatrix.Matrix[_size - 11 + i][j] = currentMatrix.Matrix[j][_size - 11 + i] = value;
            }
        }
    }

    private static void PlacePattern(BinaryMatrix matrix, ReadOnlySpan<byte> pattern, int row, int startColumn, byte amount = 8)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            PlaceBites(matrix, pattern[i], row + i, startColumn, amount);
        }
    }

    private static bool IsOverlappingFinderPattern(int x, int y, int[][] finderCoords)
    {
        foreach (var coord in finderCoords)
        {
            if (Math.Abs(coord[0] - x) <= 2 && Math.Abs(coord[1] - y) <= 2)
            {
                return true;
            }
        }
        return false;
    }

    private static void PlaceBites(BinaryMatrix matrix, byte data, int row, int startColumn, byte amount = 8)
    {
        for (var i = 0; i < amount; i++)
        {
            matrix.Matrix[row][startColumn + i] = (data & 1 << amount - 1 - i) != 0x00;
        }
    }
}
