using QrOk.Enums;

namespace QrOk.Core;

internal sealed class QrOkBuilder : IQrOkBuilder
{
    private readonly QrOkConfiguration _configuration = new();

    public IQrOkBuilder WithOutlineWidth(int outlineWidth)
    {
        _configuration.OutlineWidth = outlineWidth;
        return this;
    }

    public IQrOkBuilder WithErrorCorrectionLevel(ErrorCorrectionLevel errorCorrectionLevel)
    {
        _configuration.ErrorCorrectionLevel = errorCorrectionLevel;
        return this;
    }

    public IQrOkBuilder WithSize(Size size)
    {
        _configuration.Size = size;
        return this;
    }

    public IQrOkBuilder WithOutputPath(string path)
    {
        _configuration.OutputPath = path;
        return this;
    }

    public IQrOkBuilder From(string message)
    {
        _configuration.GetBytesAction = () => System.Text.Encoding.UTF8.GetBytes(message);
        return this;
    }

    public IQrOkBuilder From(byte[] message)
    {
        _configuration.GetBytesAction = () => message;
        return this;
    }

    public IQrOkBuilder From(Stream message)
    {
        _configuration.GetBytesAction = () =>
        {
            using MemoryStream ms = new();
            message.CopyTo(ms);
            return ms.ToArray();
        };
        return this;
    }

    public IQrOkExecutor<string> ToBase64()
    {
        return new QrOkExecutor<string>(_configuration);
    }

    public IQrOkExecutor<byte[]> ToByteArray()
    {
        return new QrOkExecutor<byte[]>(_configuration);
    }

    public IQrOkExecutor<FileInfo> ToFile(string fileName)
    {
        _configuration.FileName = fileName;
        return new QrOkExecutor<FileInfo>(_configuration);
    }

    public IQrOkExecutor<FileInfo> ToFile(string fileName, string path)
    {
        _configuration.FileName = fileName;
        _configuration.OutputPath = path;
        return new QrOkExecutor<FileInfo>(_configuration);
    }
}
