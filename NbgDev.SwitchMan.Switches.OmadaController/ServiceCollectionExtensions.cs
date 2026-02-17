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
        services.Configure<OmadaControllerOptions>(configuration);
        
        // Validate options on startup
        services.AddOptions<OmadaControllerOptions>()
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        // Register HttpClient for the Omada Controller service
        services.AddHttpClient<OmadaControllerSwitchAccessService>(client =>
        {
            // Configure default timeout and other settings
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            // For development/testing, allow self-signed certificates
            // In production, this should be properly configured
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
        
        // Register the service as ISwitchAccessService
        services.AddSingleton<ISwitchAccessService, OmadaControllerSwitchAccessService>();
        
        return services;
    }
}
