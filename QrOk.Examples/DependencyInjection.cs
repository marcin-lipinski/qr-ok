using Microsoft.Extensions.Hosting;
using QrOk.DependencyInjecton;
using QrOk.Enums;

namespace QrOk.Examples;

public static class DependencyInjection
{
    public static void ToFileUsingDIExample()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddQrOkGenerator(settings =>
        {
            settings.Size = Size.S;
            settings.OutlineWidth = 10;
            settings.ErrorCorrectionLevel = ErrorCorrectionLevel.Q;
            settings.OutputPath = "<output-path>";
        });
    }

    public class ExampleClass
    {
        private readonly IQrOkBuilder _qrBuilder;

        public ExampleClass(IQrOkBuilder qrBuilder)
        {
            _qrBuilder = qrBuilder;
        }

        public void ExampleMethod()
        {
            _qrBuilder.ToFile("filename.png");
        }
    }
}