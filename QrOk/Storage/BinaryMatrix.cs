using System.Collections;

namespace QrOk.Storage;

internal class BinaryMatrix
{
    private readonly int _size;
    private readonly Func<int, int, bool> _maskingFunction;
    private int _darkModulesCount = 0;
    public BitArray[] Matrix { get; private set; }

    public BinaryMatrix(int size, Func<int, int, bool> maskingFunction)
    {
        _size = size;
        _maskingFunction = maskingFunction;

        Matrix = new BitArray[_size];
        for (int i = 0; i < _size; i++)
        {
            Matrix[i] = new BitArray(_size, false);
        }
    }

    public void Mask(int row, int column, bool value)
    {
        Matrix[row][column] = _maskingFunction.Invoke(row, column) ? !value : value;

        if (Matrix[row][column]) _darkModulesCount++;
    }

    public int CountScore(int commonDarkModulesCount)
    {
        return CountPenaltyScoreRule1()
            + CountPenaltyScoreRule2()
            + CountPenaltyScoreRule3()
            + CountPenaltyScoreRule4(_darkModulesCount + commonDarkModulesCount);
    }

    private int CountPenaltyScoreRule1()
    {
        var score = 0;
        for (var i = 0; i < _size; i++)
        {
            var previousRowElement = Matrix[i][0];
            var previousColumnElement = Matrix[0][i];
            var rowStreak = 1;
            var colStreak = 1;

            for (var j = 1; j < _size; j++)
            {
                if (Matrix[i][j] == previousRowElement)
                {
                    rowStreak++;
                    if (rowStreak >= 5)
                    {
                        score += rowStreak == 5 ? 3 : 1;
                    }
                }
                else
                {
                    rowStreak = 1;
                    previousRowElement = Matrix[i][j];
                }

                if (Matrix[j][i] == previousColumnElement)
                {
                    colStreak++;
                    if (colStreak >= 5)
                    {
                        score += colStreak == 5 ? 3 : 1;
                    }
                }
                else
                {
                    colStreak = 1;
                    previousColumnElement = Matrix[j][i];
                }
            }
        }
        return score;
    }

    private int CountPenaltyScoreRule2()
    {
        var score = 0;
        for (var i = 0; i < _size - 1; i++)
        {
            for (var j = 0; j < _size - 1; j++)
            {
                var currVal = Matrix[i][j];
                if (currVal == Matrix[i][j + 1] && currVal == Matrix[i + 1][j] && currVal == Matrix[i + 1][j + 1])
                {
                    score += 3;
                }
            }
        }
        return score;
    }

    private int CountPenaltyScoreRule3()
    {
        int score = 0;

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size - 10; j++)
            {
                if (IsMatchingPattern(i, j, true))
                {
                    score += 40;
                }

                if (i < _size - 10 && IsMatchingPattern(i, j, false))
                {
                    score += 40;
                }
            }
        }

        return score;
    }

    private int CountPenaltyScoreRule4(int currentMatrixDarkModulesCount)
    {
        var allModulesAmount = _size * _size;
        var percentOfDark = (decimal)(currentMatrixDarkModulesCount / allModulesAmount) * 100;

        var prevMultiplyOfFive = (int)Math.Floor(percentOfDark);
        var nextMultiplyOfFive = (int)Math.Ceiling(percentOfDark);

        while (prevMultiplyOfFive % 5 != 0)
        {
            prevMultiplyOfFive--;
        }
        while (nextMultiplyOfFive % 5 != 0)
        {
            prevMultiplyOfFive++;
        }

        prevMultiplyOfFive = Math.Abs(prevMultiplyOfFive - 50);
        nextMultiplyOfFive = Math.Abs(nextMultiplyOfFive - 50);

        prevMultiplyOfFive /= 5;
        nextMultiplyOfFive /= 5;

        return Math.Min(prevMultiplyOfFive, nextMultiplyOfFive) * 10;
    }

    private bool IsMatchingPattern(int i, int j, bool isHorizontal)
    {
        int stepX = isHorizontal ? 0 : 1;
        int stepY = isHorizontal ? 1 : 0;

        return Matrix[i][j] &&
                !Matrix[i + stepX][j + stepY] &&
                Matrix[i + 2 * stepX][j + 2 * stepY] &&
                Matrix[i + 3 * stepX][j + 3 * stepY] &&
                Matrix[i + 4 * stepX][j + 4 * stepY] &&
                !Matrix[i + 5 * stepX][j + 5 * stepY] &&
                Matrix[i + 6 * stepX][j + 6 * stepY] &&
                !Matrix[i + 7 * stepX][j + 7 * stepY] &&
                !Matrix[i + 8 * stepX][j + 8 * stepY] &&
                !Matrix[i + 9 * stepX][j + 9 * stepY] &&
                !Matrix[i + 10 * stepX][j + 10 * stepY] ||

               !Matrix[i][j] &&
                !Matrix[i + stepX][j + stepY] &&
                !Matrix[i + 2 * stepX][j + 2 * stepY] &&
                !Matrix[i + 3 * stepX][j + 3 * stepY] &&
                Matrix[i + 4 * stepX][j + 4 * stepY] &&
                !Matrix[i + 5 * stepX][j + 5 * stepY] &&
                Matrix[i + 6 * stepX][j + 6 * stepY] &&
                Matrix[i + 7 * stepX][j + 7 * stepY] &&
                Matrix[i + 8 * stepX][j + 8 * stepY] &&
                !Matrix[i + 9 * stepX][j + 9 * stepY] &&
                Matrix[i + 10 * stepX][j + 10 * stepY];
    }

    public void AddOutline(int outlineWidth)
    {
        var rows = new BitArray[_size + 2 * outlineWidth];
        for (int i = 0; i < _size + 2 * outlineWidth; i++)
        {
            rows[i] = new BitArray(_size + 2 * outlineWidth, false);
        }

        for (int i = 0; i < _size; i++)
        {
            for (var j = 0; j < _size; j++)
            {
                rows[i + outlineWidth][j + outlineWidth] = Matrix[i][j];
            }
        }
        Matrix = rows;
    }

    public void Draw()
    {
        foreach (var row in Matrix)
        {
            for (var i = 0; i < _size; i++)
            {
                Console.Write(row[i] ? 1 : 0);
            }
            Console.WriteLine();
        }
    }
}
