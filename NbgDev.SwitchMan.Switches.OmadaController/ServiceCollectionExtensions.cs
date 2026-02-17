using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NbgDev.SwitchMan.Switches.Contract;

namespace NbgDev.SwitchMan.Switches.OmadaController;

/// <summary>
/// Extension methods for registering Omada Controller switch access services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Omada Controller switch access service to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance to bind options from</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOmadaControllerSwitchAccess(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configure options from configuration
        services.Configure<OmadaControllerOptions>(
            configuration.GetSection("OmadaController"));
        
        // Validate options on startup
        services.AddOptions<OmadaControllerOptions>()
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        // Build a temporary service provider to get the options for HttpClient configuration
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<OmadaControllerOptions>>().Value;
        
        // Register HttpClient for the Omada Controller service
        services.AddHttpClient<OmadaControllerSwitchAccessService>(client =>
        {
            // Configure default timeout and other settings
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            // Only bypass certificate validation if explicitly configured
            ServerCertificateCustomValidationCallback = options.AllowInvalidCertificate
                ? (message, cert, chain, errors) => true
                : null
        });
        
        // Register the service as ISwitchAccessService
        services.AddSingleton<ISwitchAccessService, OmadaControllerSwitchAccessService>();
        
        return services;
    }
}
