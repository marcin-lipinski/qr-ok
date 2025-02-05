using Microsoft.Extensions.DependencyInjection;

namespace QrOk.DependencyInjecton;

public static class Extension
{
    /// <summary>
    /// Adds the QR code generator to the collection of services. Allows to configure the settings that will be used as the default for each QR code generator.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureSettings">QR code generator configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddQrOkGenerator(this IServiceCollection services, Action<QrOkConfiguration> configureSettings)
    {
        var settings = new QrOkConfiguration();
        configureSettings(settings);

        services.AddTransient<IQrOkBuilder>(provider =>
        {
            return IQrOkBuilder.Builder
                .WithErrorCorrectionLevel(settings.ErrorCorrectionLevel)
                .WithSize(settings.Size)
                .WithOutlineWidth(settings.OutlineWidth)
                .WithOutputPath(settings.OutputPath);
        });
        return services;
    }
}
