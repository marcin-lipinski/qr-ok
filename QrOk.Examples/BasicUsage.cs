using QrOk.Enums;

namespace QrOk.Examples;

public static class BasicUsage
{
    public static void ToBase64Example()
    {
        IQrOkBuilder.Builder
            .WithErrorCorrectionLevel(ErrorCorrectionLevel.Q)
            .WithOutlineWidth(0)
            .WithSize(Size.S)
            .From("HELLO WORLD")
            .ToBase64()
            .Build();
    }

    public static void ToFileExample()
    {
        IQrOkBuilder.Builder
            .WithErrorCorrectionLevel(ErrorCorrectionLevel.L)
            .WithOutlineWidth(5)
            .WithSize(Size.M)
            .From("HELLO WORLD")
            .ToFile("filename.png", "<output-path>")
            .Build();
    }
}