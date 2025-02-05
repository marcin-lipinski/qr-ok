using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using QrOk.Enums;

namespace QrOk.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    private const string ShortNumberText = "997998999";
    private const string ShortAlphanumericText = "HELLO WORLD";
    private const string UTF8Text = "Żółć żółwia";
    private const string AverageText = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley";
    private const string LongText = "The C# language is the most popular language for the .NET platform, a free, cross-platform, open source development environment. C# programs can run on many different devices, from Internet of Things (IoT) devices to the cloud and everywhere in between.";
    private const string VeryLongText = ".NET is a free, cross-platform, open-source developer platform for building many kinds of applications. It can run programs written in multiple languages, with C# being the most popular. It relies on a high-performance runtime that is used in production by many high-scale apps.\r\n\r\nTo learn how to download .NET and start writing your first app, see Getting started.\r\n\r\nThe .NET platform has been designed to deliver productivity, performance, security, and reliability. It provides automatic memory management via a garbage collector (GC). It is type-safe and memory-safe, due to using a GC and strict language compilers. It offers concurrency via async/await and Task primitives. It includes a large set of libraries that have broad functionality and have been optimized for performance on multiple operating systems and chip architectures.\r\n\r\n.NET has the following design points:\r\n\r\nProductivity is full-stack with runtime, libraries, language, and tools all contributing to developer user experience.\r\nSafe code is the primary compute model, while unsafe code enables additional manual optimizations.\r\nStatic and dynamic code are both supported, enabling a broad set of distinct scenarios.\r\nNative code interop and hardware intrinsics are low cost and high-fidelity (raw API and instruction access).";
    private IQrOkBuilder _builder = null!;

    public static void Main()
    {
        BenchmarkRunner.Run<Benchmarks>();
    }

    [GlobalSetup]
    public void Setup()
    {
        _builder = IQrOkBuilder.Builder
            .WithOutlineWidth(4)
            .WithSize(Size.M)
            .WithOutputPath("<output-path>")
            .WithErrorCorrectionLevel(ErrorCorrectionLevel.Q);
    }

    [Benchmark]
    public void ShortNumberText_Base64()
    {
        _builder
            .From(ShortNumberText)
            .ToBase64()
            .Build();
    }

    [Benchmark]
    public void ShortNumberText_ByteArray()
    {
        _builder
            .From(ShortNumberText)
            .ToByteArray()
            .Build();
    }

    [Benchmark]
    public void ShortNumberText_File()
    {
        _builder
            .From(ShortNumberText)
            .ToFile("short_number_text_qr.png")
            .Build();
    }

    [Benchmark]
    public void ShortAlphanumericText_Base64()
    {
        _builder
            .From(ShortAlphanumericText)
            .ToBase64()
            .Build();
    }

    [Benchmark]
    public void ShortAlphanumericText_ByteArray()
    {
        _builder
            .From(ShortAlphanumericText)
            .ToByteArray()
            .Build();
    }

    [Benchmark]
    public void ShortAlphanumericText_File()
    {
        _builder
            .From(ShortAlphanumericText)
            .ToFile("short_alphanumeric_text_qr.png")
            .Build();
    }

    [Benchmark]
    public void UTF8Text_Base64()
    {
        _builder
            .From(UTF8Text)
            .ToBase64()
            .Build();
    }

    [Benchmark]
    public void UTF8Text_ByteArray()
    {
        _builder
            .From(UTF8Text)
            .ToByteArray()
            .Build();
    }

    [Benchmark]
    public void UTF8Text_File()
    {
        _builder
            .From(UTF8Text)
            .ToFile("utf8_text_qr.png")
            .Build();
    }

    [Benchmark]
    public void AverageText_Base64()
    {
        _builder
            .From(AverageText)
            .ToBase64()
            .Build();
    }

    [Benchmark]
    public void AverageText_ByteArray()
    {
        _builder
            .From(AverageText)
            .ToByteArray()
            .Build();
    }

    [Benchmark]
    public void AverageText_File()
    {
        _builder
            .From(AverageText)
            .ToFile("average_text_qr.png")
            .Build();
    }

    [Benchmark]
    public void LongText_Base64()
    {
        _builder
            .From(LongText)
            .ToBase64()
            .Build();
    }

    [Benchmark]
    public void LongText_ByteArray()
    {
        _builder
            .From(LongText)
            .ToByteArray()
            .Build();
    }

    [Benchmark]
    public void LongText_File()
    {
        _builder
            .From(LongText)
            .ToFile("long_text_qr.png")
            .Build();
    }

    [Benchmark]
    public void VeryLongText_Base64()
    {
        _builder
            .From(VeryLongText)
            .ToBase64()
            .Build();
    }

    [Benchmark]
    public void VeryLongText_ByteArray()
    {
        _builder
            .From(VeryLongText)
            .ToByteArray()
            .Build();
    }

    [Benchmark]
    public void VeryLongText_File()
    {
        _builder
            .From(VeryLongText)
            .ToFile("very_long_text_qr.png")
            .Build();
    }
}
