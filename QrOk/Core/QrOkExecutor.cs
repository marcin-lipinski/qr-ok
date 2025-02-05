using QrOk.Encoders;
using QrOk.Generators;
using QrOk.Helpers;
using QrOk.Storage;

namespace QrOk.Core;

internal sealed class QrOkExecutor<T> : IQrOkExecutor<T>
{
    private readonly QrOkConfiguration _configuration;
    private BinaryArray _binaryArray = null!;
    private byte[] _bytes = null!;

    public QrOkExecutor(QrOkConfiguration configuration)
    {
        _configuration = configuration;
    }

    public T Build()
    {
        ValidateConfiguration();

        ReadBytesFromInput();
        DefineEncodingMode();
        DefineVersion();
        InitializeBinaryArray();

        AddEncodingModeIndicator();
        AddCharacterCountIndicator();
        AddEncodedData();
        AddTerminatorAndFullfillBinaryArray();

        var transformedDate = TransformData();
        var matrix = GenerateQrCodeMatrix(transformedDate);

        return GenerateResult(matrix);
    }

    private void ValidateConfiguration()
    {
        if (typeof(T) == typeof(FileInfo))
        {
            if (string.IsNullOrWhiteSpace(_configuration.FileName))
            {
                throw new ArgumentException(nameof(_configuration.FileName), "File name not provided");
            }

            if (string.IsNullOrWhiteSpace(_configuration.OutputPath))
            {
                throw new ArgumentException(nameof(_configuration.OutputPath), "Output path name not provided");
            }
        }
    }

    private void ReadBytesFromInput()
    {
        _bytes = _configuration.GetBytesAction.Invoke();
    }

    private void DefineEncodingMode()
    {
        _configuration.Mode = BytesHelper.DefineMode(_bytes);
    }

    private void DefineVersion()
    {
        _configuration.Version = VersionHelper.DefineVersion(_configuration.Mode, _configuration.ErrorCorrectionLevel, _bytes.Length);
    }

    private void InitializeBinaryArray()
    {
        var totalCodewordsAmount = CodewordsHelper.GetTotalNumberOfCodewords(_configuration.ErrorCorrectionLevel, _configuration.Version);
        _binaryArray = new BinaryArray(totalCodewordsAmount);
    }

    private void AddEncodingModeIndicator()
    {
        var encodingModeByte = BytesHelper.GetModeByteRepresentation(_configuration.Mode);
        _binaryArray.Append(encodingModeByte, 4);
    }

    private void AddCharacterCountIndicator()
    {
        var indicatorLength = VersionHelper.GetLengthIndicator(_configuration.Version, _configuration.Mode);
        _binaryArray.Append(_bytes.Length, indicatorLength);
    }

    private void AddEncodedData()
    {
        EncodersFactory.GetEncoder(_configuration.Mode, _binaryArray).Encode(_bytes);
    }

    private void AddTerminatorAndFullfillBinaryArray()
    {
        int qrStorageInBites = _binaryArray.Length * 8;
        var difference = qrStorageInBites - _binaryArray.CurrentLength;

        if (difference > 4)
        {
            _binaryArray.Append(0x00, 4);
        }
        else if (difference > 0)
        {
            _binaryArray.Append(0x00, difference);
        }

        _binaryArray.MoveToNextByte();

        while (_binaryArray.CurrentLength < qrStorageInBites)
        {
            _binaryArray.Append(236);

            if (_binaryArray.CurrentLength < qrStorageInBites)
            {
                _binaryArray.Append(17);
            }
        }
    }

    private byte[] TransformData()
    {
        var ECPerBlock = CodewordsHelper.GetECNumber(_configuration.ErrorCorrectionLevel, _configuration.Version);
        var messagePolynomials = _binaryArray.ToPolynomials(_configuration.ErrorCorrectionLevel, _configuration.Version);
        var errorPolynomials = new byte[messagePolynomials.Length][];

        for (var i = 0; i < messagePolynomials.Length; i++)
        {
            errorPolynomials[i] = new byte[messagePolynomials[i].Length];
            messagePolynomials[i].CopyTo(errorPolynomials[i], 0);
        }

        for (int a = 0; a < errorPolynomials.Length; a++)
        {
            var generatorPolynomials = Polynomial.GetGeneratorPolynomial(ECPerBlock);
            for (int i = 0, steps = errorPolynomials[a].Length; i < steps; i++)
            {
                if (errorPolynomials[a][0] == 0)
                {
                    var polySpan = errorPolynomials[a].AsSpan();
                    errorPolynomials[a] = polySpan[1..].ToArray();
                }
                else
                {
                    int leadTermCoefficient = errorPolynomials[a][0].IntegerToExponential();
                    errorPolynomials[a] = generatorPolynomials.MultiplyCoefficient(leadTermCoefficient).XORWithPolynomial(errorPolynomials[a]);
                }
            }
        }

        return messagePolynomials.Interleave().Concat(errorPolynomials.Interleave()).ToArray();
    }

    private BinaryMatrix GenerateQrCodeMatrix(byte[] data)
    {
        var binaryMatrixBuilder = new BinaryMatrixBuilder(_configuration);

        binaryMatrixBuilder.PreMaskingActions();
        binaryMatrixBuilder.MaskData(data);

        return binaryMatrixBuilder.GetBestMatrixWithOutline();
    }

    private T GenerateResult(BinaryMatrix matrix)
    {
        using var pngStream = PNGFileGenerator.GetPNGStream(matrix.Matrix, _configuration.Size);

        if (typeof(T) == typeof(string))
        {
            return (T)(object)Convert.ToBase64String(pngStream.ToArray());
        }
        else if (typeof(T) == typeof(FileInfo))
        {
            return (T)(object)pngStream.SaveToFile(_configuration.FileName, _configuration.OutputPath);
        }

        return (T)(object)pngStream.ToArray();
    }

}