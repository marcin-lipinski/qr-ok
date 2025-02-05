# QrOk
QrOk is a free and simple QR code generator for C#. It is available as a NuGet package and also provides an optional Dependency Injection extension for easy integration into .NET applications. It is fully cross-platform.

## Installation
You can install the package via NuGet:
```sh
Install-Package QrOkGenerator
```
If you want to use the Dependency Injection extension, install the additional package:
```sh
Install-Package QrOkGenerator.DependencyInjection
```  

## Usage
### Basic usage
To generate and save a QR code as an image file, use the following code:
```csharp
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
```  

### Using Dependency Injection
If you want to configure and use the QR code generator with `IServiceCollection`, install QrOkGenerator.DependencyInjection and configure it globally:
```csharp
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
```

Once registered, you can inject `IQrOkBuilder` into your classes:
```csharp
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
```  

## [Benchmarks](https://github.com/marcin-lipinski/qr-ok/blob/master/QrOk.Benchmarks/Benchmarks.cs) results
![benchmarks](https://github.com/marcin-lipinski/qr-ok/blob/master/assets/benchmarks.png)

## License
This project is licensed under the MIT License.
