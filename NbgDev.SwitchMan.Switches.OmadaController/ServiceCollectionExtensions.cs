using Microsoft.Extensions.DependencyInjection;
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
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOmadaControllerSwitchAccess(this IServiceCollection services)
    {
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
